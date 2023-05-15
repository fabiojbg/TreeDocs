using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apps.Sdk;
using Apps.Sdk.Helpers;

namespace Apps.Sdk.SdkServices.Lock
{
    public class CacheBasedLock : IDistributedLock
    {
        class LockToken
        {
            public string Tenant { get; set; }
            public string LockKey { get; set; }
            public string LockValue { get; set; }
        }


        ICacherFactory _cacherfactory;
        ILogger _logger;

        static readonly NamedLocker _namedLocker = new NamedLocker();
        public CacheStrategy CacheStrategy { get; set; }

        public CacheBasedLock(ICacherFactory cacherfactory, ILogger<CacheBasedLock> logger)
        {
            _cacherfactory = cacherfactory;
            _logger = logger;
            CacheStrategy = CacheStrategy.Default;
        }

        public void SetCacheStrategy(CacheStrategy cacheStrategy)
        {
            CacheStrategy = cacheStrategy;
        }

        /// <summary>
        /// Implements very similar to the redlock algorithm https://redis.io/topics/distlock
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="lockName"></param>
        /// <param name="expirationInMilliseconds"></param>
        /// <param name="waitTimeoutInMilliseconds"></param>
        /// <returns></returns>
        public string LockAcquire(string tenant, string lockName, int expirationInMilliseconds, int waitTimeoutInMilliseconds)
        {  // este método não é perfeito e tem pontos de falhas mas foi feito o possível para minimizar a possibilidade de threads diferentes consigam o mesmo lock

            var start = DateTime.Now;
            _logger.LogTrace($"IN: LockAcquire. Key={lockName}");

            if (waitTimeoutInMilliseconds > 30000 || expirationInMilliseconds > 30000) // máximo de 30 segundos
                throw new Exception("Lock wait/expiration too long");
            if (expirationInMilliseconds <= 100 || waitTimeoutInMilliseconds <=100)
                throw new Exception("Lock wait/expiration timeout too short"); // minimo de 100 milisegundos

            var cacher = _cacherfactory.GetCacherFor(tenant);
            var expirationTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, 0, waitTimeoutInMilliseconds));

            if (tenant == null)
                tenant = "_null_";

            string currentLockedValue;
            var myLockValue = Guid.NewGuid().ToString();
            var lockKey = $"{nameof(CacheBasedLock)}_{lockName}";

            currentLockedValue = cacher.Get<string>(lockKey);
            int randomWait = new Random().Next(65);
            do
            {
                while (currentLockedValue != null)
                {
                    if (DateTime.UtcNow > expirationTime)
                    {
                        _logger.LogTrace($"OUT Error: LockAcquire expiration");
                        return null;
                    }
                    Thread.Sleep(10 + randomWait);

                    currentLockedValue = cacher.Get<string>(lockKey);
                }
                
                _namedLocker.RunWithLock(tenant + lockKey, () =>  // evita concorrência em um mesmo processo
                    {
                        cacher.Set(lockKey, myLockValue, new TimeSpan(0, 0, 0, 0, expirationInMilliseconds));
                        currentLockedValue = cacher.Get<string>(lockKey);  // esse é o ponto de falha pois não é possível garantir que outro processo execute a linha anterior imediatamente após a execução desta linha fazendo com que o lock seja adquirido por mais de um processo.
                    }
                );
            } while (currentLockedValue != myLockValue);

            var serializedToken = JsonConvert.SerializeObject(new LockToken { Tenant = tenant, LockKey = lockKey, LockValue = myLockValue });

            var elapsed = DateTime.Now - start;
            _logger.LogTrace($"OUT: LockAcquire. {elapsed.TotalSeconds} seconds");
            return serializedToken;
        }

        public string LockAcquire(string tenant, string lockName, int expirationInMilliseconds)
        {
            return LockAcquire(tenant, lockName, expirationInMilliseconds, expirationInMilliseconds);
        }

        public void LockRelease(string lockToken)
        {
            _logger.LogTrace($"IN: LockRelease.");
            if (lockToken == null)
            {
                _logger.LogTrace($"OUT: LockRelease1");
                return;
            }

            LockToken lockTokenDeserialized;
            try
            {
                lockTokenDeserialized = JsonConvert.DeserializeObject<LockToken>(lockToken);
            }
            catch {
                _logger.LogTrace($"OUT: LockRelease2 Error.");
                throw new Exception("Malformed lock token"); }

            var cacher = _cacherfactory.GetCacherFor(lockTokenDeserialized.Tenant == "_null_" ? null : lockTokenDeserialized.Tenant);

            _namedLocker.RunWithLock(lockTokenDeserialized.Tenant + lockTokenDeserialized.LockKey, // _namedLocker evita concorrência em um mesmo processo
                () =>  
                    {
                        var currentLockedToken = cacher.Get<string>(lockTokenDeserialized.LockKey);
                        if (currentLockedToken == lockTokenDeserialized.LockValue)
                            cacher.Delete(lockTokenDeserialized.LockKey);
                    });
            _logger.LogTrace($"OUT: LockRelease3 Key={lockTokenDeserialized.LockKey}");
        }

    }
}
