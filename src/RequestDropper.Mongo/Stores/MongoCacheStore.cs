using System;
using System.Threading.Tasks;

using MongoDB.Driver;

using RequestDropper.Mongo.Models;
using RequestDropper.Stores;

namespace RequestDropper.Mongo.Stores
{
    public class MongoCacheStore<T> : IStore<T> where T : MongoDropCounter
    {
        private readonly IMongoCollection<T> collection;
        public MongoCacheStore(IMongoDatabase database)
        {
            this.collection = database.GetCollection<T>(nameof(T));
        }
        #region Implementation of IStore<T>

        public async Task<T> GetAsync(string key)
        {
            var findCursor = await this.collection.FindAsync(Builders<T>.Filter.Eq(x => x.Key, key), new FindOptions<T> { BatchSize = 1, Limit = 1 });
            return await findCursor.FirstOrDefaultAsync();
        }

        public async Task SetAsync(string key, T entry, TimeSpan? expirationTime = null)
        {
            var updateCountDefinition = Builders<T>.Update.Set(x => x.Count, entry.Count);
            var updateTimeStampDefinition = Builders<T>.Update.Set(x => x.TimeStamp, entry.TimeStamp);
            var combinedUpdateDefinition = Builders<T>.Update.Combine(updateCountDefinition, updateTimeStampDefinition);

            await this.collection.UpdateOneAsync(Builders<T>.Filter.Eq(x => x.Key, key), combinedUpdateDefinition, new UpdateOptions { IsUpsert = true });
        }

        #endregion
    }
}
