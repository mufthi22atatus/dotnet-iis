using System;

namespace TaskManager.Services
{
    public interface ICacheService
    {
        T GetOrAdd<T>(string key, TimeSpan ttl, Func<T> factory) where T : class;
        void Remove(string key);
    }
}
