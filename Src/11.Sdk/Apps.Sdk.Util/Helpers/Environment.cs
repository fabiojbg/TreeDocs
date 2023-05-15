namespace Apps.Sdk
{ 
    public enum BuildType
    {
        Development,
        Fabric,
        Build,
        Homologation,
        Quality,
        Production
    }
    public static class EnvironmentStep
    {
        public static BuildType Current
        {
            get
            {
                return (System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) switch
                {
                    "Development" => BuildType.Development,
                    "Fabric" => BuildType.Fabric,
                    "Build" => BuildType.Build,
                    _ => BuildType.Development,
                };
            }
        }
    }
}