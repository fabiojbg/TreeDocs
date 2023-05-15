namespace Apps.Sdk
{
    public interface IDistributedLock
    {
        string LockAcquire(string tenant, string lockName, int expirationInMillisecods, int waitTimeoutInMilliseconds);
        string LockAcquire(string tenant, string lockName, int expirationInMillisecods); // expirationInMilliseconds = waitTimeoutInMilliseconds
        void LockRelease(string lockToken);
    }
}