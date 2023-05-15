using Apps.Sdk.Extensions;
using Apps.Sdk.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Domain.Shared;
using Domain.Shared.Validations;

namespace Auth.Domain.Entities

{
    public class User : Notifiable
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public string Password { get; private set; }
        public string HashedPassword { get; private set; }
        public DateTime? LastLogin { get; private set; }
        public List<string> Roles { get; private set; }

        public User(string id, string name, string userEmail, string password, string hashedPassword, params string[] roles)
        {
            if (String.IsNullOrEmpty(password) && String.IsNullOrEmpty(hashedPassword))
                AddNotification("Password", "The fields Password and HashedPassword must not be provided at the same time");
            if( String.IsNullOrEmpty(password) && String.IsNullOrEmpty(hashedPassword))
                AddNotification("Password", "Password or HashedPassword must be provided");

            Email = new Email(userEmail);
            AddNotifications(Email);

            Id = id;
            Name = name;
            Roles = roles?.ToList() ?? new List<string>();

            if (Roles?.Any(r => r.EqualsIgnoreCase(Constants.APPUSER_ROLE)) == false)
            {                
                if (Roles == null) Roles = new List<string>();
                Roles.Add(Constants.APPUSER_ROLE);
            }

            AddNotifications(new Validator()
                .HasMinLen(name, RequestsResponses.Constants.MIN_USER_NAME_LEN, "Name", Resource.ErrMinUserLen.Format(RequestsResponses.Constants.MIN_USER_NAME_LEN))
                .HasMaxLen(name, RequestsResponses.Constants.MAX_USER_NAME_LEN, "Name", Resource.ErrMaxUserLen.Format(RequestsResponses.Constants.MAX_USER_NAME_LEN))
                );

            if (!String.IsNullOrEmpty(password))
            {
                SetPassword(password);
            }
            else
                HashedPassword = hashedPassword;

        }

        public bool ValidatePassword(string password)
        {
            var passwordHash = GetHashedPassword(Id, password);

            var notifications = new Validator().AreEquals(passwordHash, HashedPassword, "Password", Resource.ErrInvalidPassword);

            AddNotifications(notifications);

            return !notifications.HasNotifications;
        }

        public void SetRoles(params string[] roles)
        {
            Roles = roles?.ToList() ?? new List<string>();
        }

        public bool SetPassword(string password)
        {
            var notifications = new Validator()
                .HasMinLen(password, RequestsResponses.Constants.MIN_PASSWORD_LEN, "Password", Resource.ErrMinPasswordLen.Format(RequestsResponses.Constants.MIN_PASSWORD_LEN))
                .HasMaxLen(password, RequestsResponses.Constants.MAX_PASSWORD_LEN, "Password", Resource.ErrMaxPasswordLen.Format(RequestsResponses.Constants.MAX_PASSWORD_LEN));

            AddNotifications(notifications);

            if (!notifications.HasNotifications)
            {
                Password = password;
                HashedPassword = GetHashedPassword(Id??"", Password);
            }

            return !notifications.HasNotifications;
        }

        public static string GetHashedPassword(string userId, string userPassword)
        {
            //return HashingHelper.ToSHA1Hash(userId + userPassword);
            return HashingHelper.ToSHA1Hash(userPassword);
        }

        public void SetId(string id)
        {
            Id = id;
        }

        public void SetLastLogin(DateTime lastLogin)
        {
            LastLogin = lastLogin;
        }

        public bool HasRole(string roleName)
        {
            if (Roles == null) return false;
            return Roles.ContainsIgnoreCase(roleName);
        }
    }
}
