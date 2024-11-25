using SportSpot.V1.WebSockets.Dtos;
using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.Session.Chat.Dtos
{
    public class MessageSendRequestDto : IWebSocketMessageDto
    {
        public WebSocketMessageType MessageType => WebSocketMessageType.MessageSendRequest;

        public required Guid SessionId { get; set; }
        public required string Message { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
