using System;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace ProxyCore
{
    internal class GenericProxyCache
    {
        private readonly MemoryCache _cache = MemoryCache.Default;

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
        {
            if (_cache.Contains(key)) return (T)_cache.Get(key);

            var value = await factory().ConfigureAwait(false);
            if (value != null)
                _cache.Add(key, value, DateTimeOffset.UtcNow.Add(ttl));

            return value;
        }
    }
}
