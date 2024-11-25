using SportSpot.V1.WebSockets.Dtos;
using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.Session.Chat.Dtos
{
    public class MessageSendResponseDto : IWebSocketMessageDto
    {
        public WebSocketMessageType MessageType => WebSocketMessageType.MessageSendResponse;

        public required MessageDto Message { get; set; }

        public bool IsToSend() => true;
    }
}
