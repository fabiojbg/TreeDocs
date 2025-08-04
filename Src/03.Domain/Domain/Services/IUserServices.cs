using Auth.Domain.Entities;

namespace TreeNotes.Domain.Services
{
    public interface IUserServices
    {
        User LoggedUser { get; }
        string LoggedUserId { get; }

        bool HasPrivilege(Privilege privilege);
    }
}