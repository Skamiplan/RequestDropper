using System;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

namespace RequestDropper.Stores
{
    /// <summary>
    /// The <see cref="DistributedCacheStore{T}"/> for RequestDropper.
    /// </summary>
    /// <typeparam name="T"><see cref="T"/></typeparam>
    public class DistributedCacheStore<T> : IStore<T> where T : struct
    {
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheStore{T}"/> class.
        /// </summary>
        /// <param name="cache"><see cref="IDistributedCache"/></param>
        public DistributedCacheStore(IDistributedCache cache)
        {
            this._cache = cache;
        }

        #region Implementation of IStore<T>

        /// <summary>
        /// Get a <see cref="T"/> by <see cref="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The <see cref="T"/>.</returns>
        public async Task<T> GetAsync(string key)
        {
            var stored = await this._cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(stored))
            {
                return JsonSerializer.Deserialize<T>(stored);
            }

            return default;
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
            var options = new DistributedCacheEntryOptions();

            if (expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(expirationTime.Value);
            }

            return this._cache.SetStringAsync(key, JsonSerializer.Serialize(entry), options);
        }

        #endregion
    }
}
