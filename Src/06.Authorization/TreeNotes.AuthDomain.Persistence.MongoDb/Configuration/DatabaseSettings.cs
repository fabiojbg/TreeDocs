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
            ConnectionString = config.GetValue<string>("TreeNotesAuthenticationDb:ConnectionString");
            if (String.IsNullOrWhiteSpace(ConnectionString))
                ConnectionString = config.GetValue<string>("TreeNotesDb:ConnectionString"); // Default to TreeNotesDb if authentication Db not defined

            DatabaseName = config.GetValue<string>("TreeNotesAuthenticationDb:DatabaseName") ?? "TreeNotesDb";
        }
    }
}