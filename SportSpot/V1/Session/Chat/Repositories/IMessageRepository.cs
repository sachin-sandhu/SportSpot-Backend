using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Entities;

namespace SportSpot.V1.Session.Chat.Repositories
{
    public interface IMessageRepository
    {
        Task CreateMessageAsync(MessageEntity message);
        Task<MessageEntity?> GetMessageAsync(Guid messageId);
        Task<(List<MessageEntity>, bool)> GetMessagesAsync(Guid sessionId, MessageSearchQueryDto searchQueryDto);
        Task UpdateMessageAsync(MessageEntity message);
        Task DeleteMessageAsync(Guid messageId);
        Task DeleteMessagesFromSession(Guid sessionId);
        Task DeleteAll();
    }
}
