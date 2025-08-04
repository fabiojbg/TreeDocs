using Auth.Domain.Entities;
using Auth.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeNotes.Domain.Services
{
    public enum Privilege
    {
        EditAnotherUserNodes,
        CreateNodes
    }



    public class UserServices : IUserServices
    {
        User _loggedUser;
        static Dictionary<Privilege, string[]> _privilegeMapping;
        public User LoggedUser => _loggedUser;
        public string LoggedUserId => _loggedUser?.Id;

        public UserServices(ILoggedUserService loggedUserService)
        {
            fillPrivilegeMappings();
            _loggedUser = loggedUserService.LoggedUser;
        }

        public bool HasPrivilege(Privilege privilege)
        {
            if (_loggedUser == null || _loggedUser.Id == "")
                return false;

            if (_privilegeMapping.ContainsKey(privilege))
            {
                var allowedRoles = _privilegeMapping[privilege];
                var allowed = (_loggedUser.Roles.Any(x => allowedRoles.Contains(x, StringComparer.InvariantCultureIgnoreCase)));
                return allowed;
            }
            return false;
        }

        static void fillPrivilegeMappings()
        {
            if (_privilegeMapping != null) return;
            _privilegeMapping = new Dictionary<Privilege, string[]>();
            _privilegeMapping.Add(Privilege.EditAnotherUserNodes, new string[] { Auth.Domain.Constants.ADMIN_USER_ROLE });
            _privilegeMapping.Add(Privilege.CreateNodes, new string[] { Auth.Domain.Constants.ADMIN_USER_ROLE, Auth.Domain.Constants.APPUSER_ROLE });
        }

    }
}
