using MongoDB.Bson;
using MongoDB.Driver;
using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Entities;

namespace SportSpot.V1.Session.Chat.Repositories
{
    public class MessageRepository(IMongoCollection<MessageEntity> _collection) : IMessageRepository
    {
        public async Task CreateMessageAsync(MessageEntity message)
        {
            await _collection.InsertOneAsync(message);
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            await _collection.DeleteOneAsync(x => x.Id == messageId);
        }

        public async Task<MessageEntity?> GetMessageAsync(Guid messageId)
        {
            IAsyncCursor<MessageEntity> cursor = await _collection.FindAsync(x => x.Id == messageId);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<(List<MessageEntity>, bool)> GetMessagesAsync(Guid sessionId, MessageSearchQueryDto searchQueryDto)
        {
            FindOptions<MessageEntity> options = new()
            {
                Limit = searchQueryDto.Size,
                Skip = searchQueryDto.Page * searchQueryDto.Size
            };

            FilterDefinitionBuilder<MessageEntity> filterBuilder = Builders<MessageEntity>.Filter;
            List<FilterDefinition<MessageEntity>> andFiler = [];

            andFiler.Add(filterBuilder.Eq(x => x.SessionId, sessionId));

            // Custom and Filters
            if (searchQueryDto.SenderId.HasValue)
                andFiler.Add(filterBuilder.Eq(x => x.CreatorId, searchQueryDto.SenderId.Value));
            if (!string.IsNullOrEmpty(searchQueryDto.Content) && !string.IsNullOrWhiteSpace(searchQueryDto.Content))
                andFiler.Add(filterBuilder.Regex(x => x.Content, new BsonRegularExpression(searchQueryDto.Content, "i")));
            if (searchQueryDto.StartTime.HasValue)
                andFiler.Add(filterBuilder.Gte(x => x.CreatedAt, searchQueryDto.StartTime.Value));
            if (searchQueryDto.EndTime.HasValue)
                andFiler.Add(filterBuilder.Lte(x => x.CreatedAt, searchQueryDto.EndTime.Value));

            FilterDefinition<MessageEntity> finalFilter = filterBuilder.And(andFiler);
            IAsyncCursor<MessageEntity> cursor = await _collection.FindAsync(finalFilter, options);
            List<MessageEntity> messages = await cursor.ToListAsync();
            if (messages.Count < searchQueryDto.Size)
                return (messages, false);

            long allDocumentCount = await _collection.CountDocumentsAsync(finalFilter);

            bool hasMoreEntries = allDocumentCount > (searchQueryDto.Size + (searchQueryDto.Page * searchQueryDto.Size));
            return (messages, hasMoreEntries);
        }

        public async Task UpdateMessageAsync(MessageEntity message)
        {
            await _collection.ReplaceOneAsync(x => x.Id == message.Id, message);
        }

        public async Task DeleteMessagesFromSession(Guid sessionId)
        {
            await _collection.DeleteManyAsync(x => x.SessionId == sessionId);
        }

        public async Task DeleteAll()
        {
            BsonDocument filter = [];
            await _collection.DeleteManyAsync(filter);
        }
    }
}
