using Chain.Interfaces;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Chain.Infrastructure
{
    public class MemoryStorage<T> : IWritableStorageProvider<T> where T : class
    {
        private readonly MemoryCache Cache;
        private TimeSpan expirationTime;
        private readonly string Key;
        private readonly object LockObj = new object();
        public MemoryStorage(string key, TimeSpan expirationTime)
        {
            Cache = MemoryCache.Default;
            this.expirationTime = expirationTime;
            this.Key = key;
        }
        public TimeSpan ExpirationTime { get => expirationTime; set => expirationTime = value; }
        public Task<T> Read()
        {
            lock (LockObj)
            {
                if (!string.IsNullOrEmpty(Key) && Cache.Contains(Key))
                {
                    var result = Cache.Get(Key);
                    return Task.FromResult((T)result);
                }
                return Task.FromResult<T>(null);
            }
        }

        public Task Write(T value)
        {
            lock (LockObj)
            {
                if (!Cache.Contains(Key))
                {
                    var policy = new CacheItemPolicy { SlidingExpiration = expirationTime };
                    Cache.Add(Key, value, policy);
                    Console.WriteLine("MemoryStorage stored new item");
                }
                return Task.CompletedTask;
            }
        }
    }
}
