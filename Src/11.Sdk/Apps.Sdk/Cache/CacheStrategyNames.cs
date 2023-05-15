using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.Cache
{
    public static class CacheStrategyNames
    {
        public const string MONGO = "MongoCache";
        public const string REDIS = "RedisCache";
        public const string MEMORY = "ProcessMemory";
    }
}
