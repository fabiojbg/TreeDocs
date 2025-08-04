using Apps.Sdk.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Auth.Domain.Repository;
using Auth.Domain.Entities;
using Domain.Shared;

namespace Auth.Domain.Services
{
    public enum Privilege
    {
        CreateUser,
        UpdateUser,
        SetUserPassword,
        UpdateUserPassword,
        GetAnyUserData
    }

    public class LoggedUserService : ILoggedUserService
    {
        IAppUserService _appUserService;
        IUserRepository _userRepository;
        ILogger<LoggedUserService> _logger;
        User _loggedUser;
        static Dictionary<Privilege, string[]> _privilegeMapping;

        public LoggedUserService(IAppUserService appUserService, IUserRepository userRepository, ILogger<LoggedUserService> logger)
        {
            _appUserService = appUserService;
            _userRepository = userRepository;
            _logger = logger;
            fillPrivilegeMappings();
        }

        public User LoggedUser
        {
            get
            {
                var loggedUserId = _appUserService.GetLoggedUserId();

                if (_loggedUser != null && _loggedUser.Id == loggedUserId)
                    return _loggedUser;

               // try
                {
                    _loggedUser = AsyncHelper.RunSync( ()=> _userRepository.GetUserById(loggedUserId));
                    if (_loggedUser == null)
                    {
                        _loggedUser = new User("", "", null, "", "");
                        return null;
                    }
                    return _loggedUser;
                }
                //catch (Exception ex)
                //{
                //    _logger.LogDebug(ex, "Error getting user with id={0} from repository");
                //    _loggedUser = new User("", "", null, "", "");
                //    return null;
                //}
            }
        }

        public bool HasPrivilege(Privilege privilege)
        {
            if (LoggedUser == null || LoggedUser.Id == "")
                return false;

            if (_privilegeMapping.ContainsKey(privilege))
            {
                var allowedRoles = _privilegeMapping[privilege];
                var allowed = (LoggedUser.Roles.Any(x => allowedRoles.Contains(x, StringComparer.InvariantCultureIgnoreCase)));
                return allowed;
            }
            return false;
        }

        static void fillPrivilegeMappings()
        {
            if (_privilegeMapping != null) return;
            _privilegeMapping = new Dictionary<Privilege, string[]>();
            _privilegeMapping.Add(Privilege.CreateUser, new string[] { Constants.ADMIN_USER_ROLE } );
            _privilegeMapping.Add(Privilege.GetAnyUserData, new string[] { Constants.ADMIN_USER_ROLE });
            _privilegeMapping.Add(Privilege.SetUserPassword, new string[] { Constants.ADMIN_USER_ROLE });
            _privilegeMapping.Add(Privilege.UpdateUser, new string[] { Constants.ADMIN_USER_ROLE });
            _privilegeMapping.Add(Privilege.UpdateUserPassword, new string[] { Constants.ADMIN_USER_ROLE });
        }

        public bool HasRole(string roleName)
        {
            if (LoggedUser == null) return false;
            return LoggedUser.HasRole(roleName);
        }

    }
}
