using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeDocs.Service.Contracts.Authentication;

namespace TreeDocs.ClientApp.Services
{
    public class AppState
    {
        ISyncLocalStorageService _localStorageService;
        public AppState(ISyncLocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public event Action OnChange;

        public AuthenticatedUser AuthenticatedUser
        {
            get
            {
                var authenticatedUser = _localStorageService.GetItem<AuthenticatedUser>("NoterUser");
                return authenticatedUser;
            }
            private set
            {

                _localStorageService.SetItem("NoterUser", value);
            }
        }

        public void SetAuthenticatedUser(AuthenticatedUser authenticatedUser)
        {
            AuthenticatedUser = authenticatedUser;
            OnChange?.Invoke();
        }
    }
}
