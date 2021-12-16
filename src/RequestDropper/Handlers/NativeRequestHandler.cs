using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using RequestDropper.Models;
using RequestDropper.Stores;

namespace RequestDropper.Handlers
{
    /// <summary>
    /// The <see cref="NativeRequestHandler"/> for RequestDropper.
    /// </summary>
    public class NativeRequestHandler : RequestHandler<DropCounter?>
    {
        private readonly IStore<DropCounter?> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeRequestHandler"/> class.
        /// </summary>
        /// <param name="store">The <see cref="IStore{T}"/> to store the <see cref="DropCounter"/>.</param>
        /// <param name="dropperSettings">The <see cref="DropperSettings"/> to match the requests again.</param>
        public NativeRequestHandler(IStore<DropCounter?> store, IOptions<DropperSettings> dropperSettings)
            : base(dropperSettings)
        {
            this._store = store;
        }

        #region Overrides of RequestHandler<DropCounter>

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="key"><see cref="key"/> to find a <see cref="DropCounter"/>.</param>
        /// <returns>The <see cref="DropCounter"/>.</returns>
        public override async Task<DropCounter?> HandleRequestAsync(string key)
        {
            var entry = await this._store.GetAsync(key);

            if (entry.HasValue)
            {
                if (entry.Value.TimeStamp + base.GetPeriod() <= DateTimeOffset.Now)
                {
                    var counter = new DropCounter(0, DateTimeOffset.Now);
                    await this._store.SetAsync(key, counter);
                }
            }

            return entry;
        }

        /// <summary>
        /// Increase the <see cref="DropCounter"/> for <see cref="key"/> due to rule <see cref="rule"/> being violated.
        /// </summary>
        /// <param name="rule">The <see cref="rule"/> the request has violated.</param>
        /// <param name="key">The <see cref="key"/> that voilated the <see cref="rule"/>.</param>
        /// <returns></returns>
        public override async Task IncrementCounterAsync(DropperRule rule, string key)
        {
            var entry = await this._store.GetAsync(key);

            if (entry.HasValue)
            {
                entry = new DropCounter(entry.Value.Count + rule.Weight, entry.Value.TimeStamp);
            }
            else
            {
                entry = new DropCounter(rule.Weight, DateTimeOffset.Now);
            }

            await this._store.SetAsync(key, entry, base.GetPeriod());
        }

        /// <summary>
        /// Check if the quotaExceeded
        /// </summary>
        /// <param name="counter">The <see cref="DropCounter"/> to be checked.</param>
        /// <returns>True if the <see cref="DropCounter"/> exceeded the limit given in <see cref="DropperSettings"/>.</returns>
        public override Task<bool> QuotaExceededAsync(DropCounter? counter)
        {
            if (counter.HasValue && counter.Value.Count >= base.GetLimit())
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        #endregion
    }
}
