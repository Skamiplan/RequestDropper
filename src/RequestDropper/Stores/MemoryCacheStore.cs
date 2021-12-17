using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

namespace RequestDropper.Stores
{
    /// <summary>
    /// The <see cref="MemoryCacheStore{T}"/> for RequestDropper.
    /// </summary>
    /// <typeparam name="T"><see cref="T"/></typeparam>
    internal class MemoryCacheStore<T> : IStore<T> 
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheStore{T}"/> class.
        /// </summary>
        /// <param name="cache"><see cref="IMemoryCache"/></param>
        public MemoryCacheStore(IMemoryCache cache)
        {
            this._cache = cache;
        }

        #region Implementation of IStore<T>

        /// <summary>
        /// Get a <see cref="T"/> by <see cref="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The <see cref="T"/>.</returns>
        public Task<T> GetAsync(string key)
        {
            if (this._cache.TryGetValue(key, out T stored))
            {
                return Task.FromResult(stored);
            }

            return Task.FromResult(default(T))!;
        }

        /// <summary>
        /// Sets the <see cref="T"/> in <see cref="IStore{T}"/>
        /// </summary>
        /// <param name="key">The <see cref="key"/> that belongs to the <see cref="T"/></param>
        /// <param name="entry"><see cref="T"/></param>
        /// <param name="expirationTime">The <see cref="expirationTime"/> that the <see cref="T"/> should be counted for.</param>
        /// <returns><see cref="Task"/></returns>
        public Task SetAsync(string key, T entry, TimeSpan? expirationTime = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            };

            if (expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(expirationTime.Value);
            }
            this._cache.Set(key, entry, options);

            return Task.CompletedTask;
        }

        #endregion
    }
}
