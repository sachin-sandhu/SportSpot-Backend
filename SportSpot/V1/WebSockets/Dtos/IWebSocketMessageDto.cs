using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.WebSockets.Dtos
{
    public interface IWebSocketMessageDto
    {
        WebSocketMessageType MessageType { get; }
    }
}
