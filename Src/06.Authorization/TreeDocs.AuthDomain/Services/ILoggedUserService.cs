using Auth.Domain.Entities;
using System.Threading.Tasks;

namespace Auth.Domain.Services
{
    public interface ILoggedUserService
    {
        User LoggedUser { get; }
        bool HasPrivilege(Privilege privilege);
        bool HasRole(string roleName);
    }
}