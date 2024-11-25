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
        public async Task<List<MessageDto>> GetMessages(SessionEntity session)
        {
            List<MessageEntity> messages = await _repository.GetMessagesAsync(session.Id);
            List<MessageDto> messageDtos = messages.Select(x => x.ConvertToDto()).ToList();
            return messageDtos;
        }

        public async Task HandleMessage(MessageSendRequestDto requestDto, AuthUserEntity sender)
        {
            SessionEntity session = await _sessionService.Get(requestDto.SessionId);
            MessageEntity messageEntity = new()
            {
                Id = Guid.CreateVersion7(),
                SessionId = session.Id,
                CreatorId = sender.Id,
                Message = requestDto.Message,
                ParentMessageId = requestDto.ParentMessageId,
                CreatedAt = DateTime.Now
            };
            await _repository.CreateMessageAsync(messageEntity);

            List<Guid> userToSend = [];
            userToSend.AddRange(session.Participants);
            userToSend.Add(session.CreatorId);

            MessageDto messageDto = messageEntity.ConvertToDto();
            MessageSendResponseDto responseDto = new MessageSendResponseDto
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
