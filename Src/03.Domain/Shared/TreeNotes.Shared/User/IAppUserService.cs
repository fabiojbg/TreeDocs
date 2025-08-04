using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Shared
{
    public interface IAppUserService
    {
        string GetLoggedUserId();
        string GetLoggedUserName();
    }
}
