using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.MongoDb
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public DatabaseSettings(IConfiguration config)
        {
            ConnectionString = config.GetValue<string>("TreeNotesDb:ConnectionString");
            DatabaseName = config.GetValue<string>("TreeNotesDb:DatabaseName");
        }
    }
}