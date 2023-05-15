namespace MongoDb.Common
{ 
    public interface IDatabaseSettings
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }
}