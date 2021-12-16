using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RequestDropper.Stores
{
    /// <summary>
    /// The <see cref="InMemoryStore{T}"/> for RequestDropper.
    /// </summary>
    /// <typeparam name="T"><see cref="T"/></typeparam>
    public class InMemoryStore<T> : IStore<T>
        where T : struct
    {
        private readonly ConcurrentDictionary<string, T> _counterDictionary = new();

        #region Implementation of IStore

        /// <summary>
        /// Get a <see cref="T"/> by <see cref="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The <see cref="T"/>.</returns>
        public Task<T> GetAsync(string key)
        {
            this._counterDictionary.TryGetValue(key, out var counter);
            return Task.FromResult(counter);
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
            this._counterDictionary.AddOrUpdate(key, entry, (x, oldEntry) => entry);
            return Task.CompletedTask;
        }

        #endregion
    }
}
