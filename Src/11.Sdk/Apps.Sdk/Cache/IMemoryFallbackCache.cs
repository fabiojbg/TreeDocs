using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sdk.SdkServices
{

    public interface IMemoryFallbackCache : IDistributedCacher
    {
        void SetFallbackParameters(CacheStrategy cacheStrategy, TimeSpan memoryFallbackTime);
    }
}
