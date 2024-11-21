using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.Repositories
{
    public class MediaRepository(IMongoCollection<MediaEntity> _collection) : IMediaRepository
    {
        public async Task Add(MediaEntity medium)
        {
            await _collection.InsertOneAsync(medium);
        }

        public async Task Delete(MediaEntity medium)
        {
            await _collection.DeleteOneAsync(x => x.Id == medium.Id);
        }

        public async Task Update(MediaEntity medium)
        {
            await _collection.ReplaceOneAsync(x => x.Id == medium.Id, medium);
        }

        public async Task<MediaEntity?> Get(Guid id)
        {
            IAsyncCursor<MediaEntity> cursor = await _collection.FindAsync(x => x.Id == id);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<List<MediaEntity>> GetAll()
        {
            BsonDocument filter = [];
            IAsyncCursor<MediaEntity> cursor = await _collection.FindAsync(filter);
            return await cursor.ToListAsync();
        }
    }
}
