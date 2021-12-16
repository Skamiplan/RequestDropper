using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using RequestDropper.Handlers;
using RequestDropper.Models;
using RequestDropper.Mongo.Models;
using RequestDropper.Stores;

namespace RequestDropper.Mongo.Handlers
{
    public class MongoRequestHandler : RequestHandler<MongoDropCounter?>
    {
        private readonly IStore<MongoDropCounter?> store;

        public MongoRequestHandler(IStore<MongoDropCounter?> store, IOptions<DropperSettings> dropperSettings)
            : base(dropperSettings)
        {
            this.store = store;
        }

        #region Overrides of RequestHandler<MongoDropCounter>

        public override async Task<MongoDropCounter?> HandleRequestAsync(string key)
        {
            var entry = await this.store.GetAsync(key) ?? new MongoDropCounter();

            if (entry.TimeStamp + base.GetPeriod() <= DateTimeOffset.Now)
            {
                var counter = new MongoDropCounter { TimeStamp = DateTimeOffset.Now, Count = 0 };
                await this.store.SetAsync(key, counter);
            }

            return entry;
        }

        public override async Task IncrementCounterAsync(DropperRule rule, string key)
        {
            var entry = await this.store.GetAsync(key);

            if (entry != null)
            {
                entry = new MongoDropCounter { Count = entry.Count + rule.Weight, TimeStamp = entry.TimeStamp };
            }
            else
            {
                entry = new MongoDropCounter { Count = rule.Weight, TimeStamp = DateTimeOffset.Now };
            }

            await this.store.SetAsync(key, entry, base.GetPeriod());
        }

        public override Task<bool> QuotaExceededAsync(MongoDropCounter? counter)
        {
            if (counter != null && counter.TimeStamp + base.GetPeriod() >= DateTimeOffset.Now && counter.Count >= base.GetLimit())
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        #endregion
    }
}
