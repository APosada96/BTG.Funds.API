
using BTG.Funds.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BTG.Funds.Infrastructure.Repositories
{
    public class MongoRepository<T> : IRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<List<T>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<T?> GetByIdAsync(string id)
        {
            var filter = BuildIdFilter(id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task AddAsync(T entity) => await _collection.InsertOneAsync(entity);
        public async Task UpdateAsync(string id, T entity)
        {
            FilterDefinition<T> filter;
            if (ObjectId.TryParse(id, out var objectId))
                filter = Builders<T>.Filter.Eq("_id", objectId);
            else
                filter = Builders<T>.Filter.Eq("_id", id);

            await _collection.ReplaceOneAsync(filter, entity);
        }
        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));

        private static FilterDefinition<T> BuildIdFilter(string id)
        {
            if (ObjectId.TryParse(id, out var objectId))
                return Builders<T>.Filter.Eq("_id", objectId);

            return Builders<T>.Filter.Eq("_id", id);
        }
    }
}
