using MongoDB.Driver;
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

        public async Task<List<MessageEntity>> GetMessagesAsync(Guid sessionId)
        {
            IAsyncCursor<MessageEntity> cursor = await _collection.FindAsync(x => x.SessionId == sessionId);
            List<MessageEntity> messages = await cursor.ToListAsync();
            return messages;
        }

        public async Task UpdateMessageAsync(MessageEntity message)
        {
            await _collection.ReplaceOneAsync(x => x.Id == message.Id, message);
        }

        public async Task DeleteMessagesFromSession(Guid sessionId)
        {
            await _collection.DeleteManyAsync(x => x.SessionId == sessionId);
        }
    }
}
