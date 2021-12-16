using System;
using System.Threading.Tasks;

namespace RequestDropper.Stores
{
    public interface IStore<T>
    {
        /// <summary>
        /// Get a <see cref="T"/> by <see cref="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The <see cref="T"/>.</returns>
        Task<T> GetAsync(string key);

        /// <summary>
        /// Sets the <see cref="T"/> in <see cref="IStore{T}"/>
        /// </summary>
        /// <param name="key">The <see cref="key"/> that belongs to the <see cref="T"/></param>
        /// <param name="entry"><see cref="T"/></param>
        /// <param name="expirationTime">The <see cref="expirationTime"/> that the <see cref="T"/> should be counted for.</param>
        /// <returns><see cref="Task"/></returns>
        Task SetAsync(string key, T entry, TimeSpan? expirationTime = null);
    }
}
