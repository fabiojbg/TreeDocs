using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth.Domain;
using TreeDocs.Service.Contracts;
using TreeDocs.Service.Contracts.Authentication;
using Apps.Sdk.Helpers;
using Apps.Sdk;
using Apps.Sdk.DependencyInjection;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;

namespace TreeDocs.Service.Authorization
{
    public class TokenData
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }

    public static class TokenService
    {
        enum TokenEncodeType
        {
            Caching,
            Encryption
        }

        const string ENCRYPTED_PREFIX = "e.";
        const string CACHED_PREFIX = "c.";

        public static byte[] GetSecretKey(IConfiguration config)
        {
            if (config.GetValue<string>("AuthToken:SecretKey") == null)
                return Encoding.ASCII.GetBytes("0668dbd3-6f0e-4cb0-bdf9-a5e64d9d9963");

            return Encoding.ASCII.GetBytes(config.GetValue<string>("AuthToken:SecretKey"));
        }

        public static byte[] GetEncryptionKey(IConfiguration config)
        {
            if (config.GetValue<string>("AuthToken:EncryptionKey") == null)
                return null;

            return Encoding.ASCII.GetBytes(config.GetValue<string>("AuthToken:EncryptionKey"));
        }

        public static TokenData GenerateToken(AuthenticatedUser user, byte[] secretKeyBytes, byte[] encryptKeyBytes, TimeSpan expiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(secretKeyBytes);
            var encryptKey = encryptKeyBytes == null ? null : new SymmetricSecurityKey(encryptKeyBytes);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            foreach(var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration),
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = encryptKey==null ? null : new EncryptingCredentials(encryptKey, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512),
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var encodedToken = EncodeToken(jwtToken, TokenEncodeType.Caching, expiration);

            return new TokenData { Token = encodedToken, Expiration = tokenDescriptor.Expires.Value };
        }

        public static string DecodeToken(string token)
        {
            string resolvedToken = token;

            try
            {
                if (token.StartsWith(CACHED_PREFIX)) // indicates cached token
                {
                    var tokenKey = token.Substring(CACHED_PREFIX.Length).Trim();
                    resolvedToken = TokenService.GetTokenFromCache(tokenKey);
                    if (resolvedToken != null)
                        return DecodeToken(resolvedToken);
                }
                else
                if (token.StartsWith(ENCRYPTED_PREFIX)) // indicates encrypted token
                {
                    var encryptedToken = token.Substring(ENCRYPTED_PREFIX.Length).Trim();
                    resolvedToken = TokenService.DecryptToken(encryptedToken);
                    return DecodeToken(resolvedToken);
                }
            }
            catch(Exception ex)
            {
                var logger = SdkDI.Resolve<ILogger<object>>();
                if (logger != null)
                    logger.LogWarning(ex, "TokenService: DecodeToken error");
            }

            return resolvedToken;
        }

        private static string EncodeToken(string token, TokenEncodeType encodeType, TimeSpan expirationTime)
        {
            if( encodeType == TokenEncodeType.Caching)
            {
                var cacheKey = SetTokenInCache(token, expirationTime);
                return CACHED_PREFIX + cacheKey; // the token is now the cacheKey
            }

            if (encodeType == TokenEncodeType.Encryption)
            {
                var encryptedToken = EncryptToken(token);
                return ENCRYPTED_PREFIX + encryptedToken;
            }

            return token;
        }

        public static AuthenticatedUser GetAuthenticatedUser(ClaimsPrincipal userClaims)
        {
            var user = new AuthenticatedUser
            {
                Id = userClaims?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                Username = userClaims?.Identity?.Name,
                Email = userClaims?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Roles = userClaims?.Claims?.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray()
            };
            return user;
        }

        private static string SetTokenInCache(string tokenValue, TimeSpan expiration)
        {
            var cachedKey = Base64.ToBase64(Guid.NewGuid().ToString());

            var cacher = SdkDI.Resolve<IDistributedCacher>();
            cacher.Set("UserToken." + cachedKey, EncryptToken(tokenValue), expiration);
            //cacher.Set("UserToken." + cachedKey, tokenValue, expiration);
            return cachedKey;
        }

        private static string GetTokenFromCache(string tokenKey)
        {
            var cacher = SdkDI.Resolve<IDistributedCacher>();
            var encrypted = cacher.Get<string>("UserToken." + tokenKey);
            if (encrypted == null)
                return null;
            var decrypted = DecryptToken(encrypted);
            //var decrypted = encrypted;
            return decrypted;
        }

        private static string EncryptToken(string token)
        {
            var res = Cryptography.EncryptAESToString(token, "Tr33D0cs");
            return res;
        }

        private static string DecryptToken(string token)
        {
            var res = Cryptography.DecryptAESToString(token, "Tr33D0cs");
            return res;
        }

    }
}
