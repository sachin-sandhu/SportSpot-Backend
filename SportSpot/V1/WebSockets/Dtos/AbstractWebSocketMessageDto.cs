using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.WebSockets.Dtos
{
    public abstract class AbstractWebSocketMessageDto
    {
        public WebSocketMessageType MessageType { get; set; }
    }
}
