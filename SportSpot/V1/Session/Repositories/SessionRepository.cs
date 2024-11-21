using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;

namespace SportSpot.V1.Session.Repositories
{
    public class SessionRepository(IMongoCollection<SessionEntity> _collection) : ISessionRepository
    {
        public async Task Add(SessionEntity sessionEntity)
        {
            await _collection.InsertOneAsync(sessionEntity);
        }

        public async Task DeleteSession(SessionEntity sessionEntity)
        {
            await _collection.DeleteOneAsync(x => x.Id == sessionEntity.Id);
        }

        public async Task UpdateSession(SessionEntity sessionEntity)
        {
            await _collection.ReplaceOneAsync(x => x.Id == sessionEntity.Id, sessionEntity);
        }

        public async Task<SessionEntity?> GetSession(Guid sessionId)
        {
            IAsyncCursor<SessionEntity> cursor = await _collection.FindAsync(x => x.Id == sessionId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<List<SessionEntity>> GetAll()
        {
            BsonDocument filter = [];
            IAsyncCursor<SessionEntity> cursor = await _collection.FindAsync(filter);
            return await cursor.ToListAsync();
        }

        public async Task<(List<SessionEntity>, bool)> GetSessionsInRange(SessionSearchQueryDto requestDto, Guid userID)
        {
            FilterDefinitionBuilder<SessionEntity> filterBuilder = Builders<SessionEntity>.Filter;
            FindOptions<SessionEntity> options = new()
            {
                Skip = requestDto.Page * requestDto.Size,
                Limit = requestDto.Size
            };

            // Radius
            double radiusInDegrees = requestDto.Distance / 6378.1;

            List<FilterDefinition<SessionEntity>> filter = [];
            filter.Add(filterBuilder.Ne(x => x.CreatorId, userID));
            filter.Add(filterBuilder.Not(filterBuilder.AnyEq(x => x.Participants, userID)));
            filter.Add(filterBuilder.Gt(x => x.Date, DateTime.Now));
            filter.Add(filterBuilder.GeoWithinCenterSphere(x => x.Location.Coordinates, requestDto.Longitude, requestDto.Latitude, radiusInDegrees));

            if (requestDto.SportType != null)
                filter.Add(filterBuilder.Eq(x => x.SportType, requestDto.SportType.Value));

            FilterDefinition<SessionEntity> finalFilter = filterBuilder.And(filter);

            IAsyncCursor<SessionEntity> cursor = await _collection.FindAsync(finalFilter, options);
            List<SessionEntity> result = await cursor.ToListAsync();

            if (result.Count < requestDto.Size)
                return (result, false);

            long fullResultCount = await _collection.CountDocumentsAsync(finalFilter);
            bool hasMoreEntries = fullResultCount > (requestDto.Size + (requestDto.Size * requestDto.Page));

            return (result, hasMoreEntries);
        }
    }
}
