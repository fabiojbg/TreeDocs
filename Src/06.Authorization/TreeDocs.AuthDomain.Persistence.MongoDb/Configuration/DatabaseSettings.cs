using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Persistence.MongoDb
{ 
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public DatabaseSettings(IConfigurationRoot config)
        {
            ConnectionString = config.GetValue<string>("NoterAuthenticationDb:ConnectionString");
            DatabaseName = config.GetValue<string>("NoterAuthenticationDb:DatabaseName");
        }
    }
}