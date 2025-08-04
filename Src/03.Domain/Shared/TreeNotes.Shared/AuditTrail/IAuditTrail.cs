


namespace Domain.Shared

{
    public interface IAuditTrail
    {
        void InsertEntry(string message, string userName = null, string userId = null, string userIP = null, string messageDetails = null);
    }
}