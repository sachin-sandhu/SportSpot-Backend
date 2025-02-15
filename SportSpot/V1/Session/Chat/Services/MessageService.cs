using SportSpot.V1.Exceptions.Session;
using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Entities;
using SportSpot.V1.Session.Chat.Mapper;
using SportSpot.V1.Session.Chat.Repositories;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.Session.Services;
using SportSpot.V1.User.Entities;
using SportSpot.V1.WebSockets.Services;

namespace SportSpot.V1.Session.Chat.Services
{
    public class MessageService(IWebSocketService _webSocketService, ISessionService _sessionService, IMessageRepository _repository) : IMessageService
    {
        public async Task DeleteAll()
        {
            await _repository.DeleteAll();
        }

        public async Task DeleteAll(SessionEntity session)
        {
            await _repository.DeleteMessagesFromSession(session.Id);
        }

        public async Task DeleteAll(AuthUserEntity user)
        {
            await _repository.DeleteAll(user);
        }

        public async Task<(List<MessageDto>, bool)> GetMessages(SessionEntity session, MessageSearchQueryDto searchQueryDto, AuthUserEntity authUserEntity)
        {
            if (!_sessionService.IsMember(session, authUserEntity))
                throw new SessionNotJoinedException();
            if (searchQueryDto.Page < 0 || searchQueryDto.Size <= 0 || searchQueryDto.Size > 1000)
                throw new SessionInvalidPageException();
            (List<MessageEntity> messages, bool hasMoreEntries) = await _repository.GetMessagesAsync(session.Id, searchQueryDto);
            List<MessageDto> messageDtos = messages.Select(x => x.ConvertToDto()).ToList();
            return (messageDtos, hasMoreEntries);
        }

        public async Task HandleMessage(MessageSendRequestDto requestDto, AuthUserEntity sender)
        {
            SessionEntity session = await _sessionService.Get(requestDto.SessionId);
            MessageEntity messageEntity = new()
            {
                Id = Guid.CreateVersion7(),
                SessionId = session.Id,
                CreatorId = sender.Id,
                Content = requestDto.Content,
                ParentMessageId = requestDto.ParentMessageId,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateMessageAsync(messageEntity);

            List<Guid> userToSend = [];
            userToSend.AddRange(session.Participants);
            userToSend.Add(session.CreatorId);

            MessageDto messageDto = messageEntity.ConvertToDto();
            MessageSendResponseDto responseDto = new()
            {
                Message = messageDto
            };

            foreach (Guid userId in userToSend)
            {
                await _webSocketService.SendMessage(userId, responseDto);
            }
        }
    }
}
