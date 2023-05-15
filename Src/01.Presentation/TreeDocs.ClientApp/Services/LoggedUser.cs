using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeDocs.Service.Contracts.Authentication;

namespace TreeDocs.ClientApp.Services
{
    public interface eIUserService
    {
        AuthenticatedUser AuthenticatedUser { get; set; }
    }

    public class eUserService : eIUserService
    {
        ISyncLocalStorageService _localStorageService;
        public eUserService(ISyncLocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public AuthenticatedUser AuthenticatedUser
        {
            get
            {
                var authenticatedUser = _localStorageService.GetItem<AuthenticatedUser>("NoterUser");
                return authenticatedUser;
            }
            set
            {

                _localStorageService.SetItem("NoterUser", value);
            }
        }

    }
}
