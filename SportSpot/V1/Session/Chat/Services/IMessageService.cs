using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Session.Chat.Services
{
    public interface IMessageService
    {
        Task HandleMessage(MessageSendRequestDto requestDto, AuthUserEntity sender);
        Task<(List<MessageDto>, bool)> GetMessages(SessionEntity session, MessageSearchQueryDto searchQueryDto, AuthUserEntity authUserEntity);
        Task DeleteAll();
        Task DeleteAll(SessionEntity session);
        Task DeleteAll(AuthUserEntity user);
    }
}
