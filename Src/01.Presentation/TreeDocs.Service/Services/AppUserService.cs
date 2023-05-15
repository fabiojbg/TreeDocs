using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Auth.Domain.Services;
using Domain.Shared;

namespace TreeDocs.Service.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetLoggedUserId()
        {
            var result =  _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)??"-";
            return result;
        }

        public string GetLoggedUserName()
        {
            var result =  _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)??"-";
            return result;
        }
    }
}
