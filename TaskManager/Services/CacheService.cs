using System;
using System.Runtime.Caching;

namespace TaskManager.Services
{
    public class CacheService : ICacheService
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public T GetOrAdd<T>(string key, TimeSpan ttl, Func<T> factory) where T : class
        {
            if (_cache.Get(key) is T cached) return cached;

            var value = factory();
            if (value == null) return null;

            _cache.Set(key, value, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.Add(ttl)
            });
            return value;
        }

        public void Remove(string key) => _cache.Remove(key);
    }
}
