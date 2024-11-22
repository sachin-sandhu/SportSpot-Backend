using SportSpot.V1.User.Entities;
using SportSpot.V1.WebSockets.Dtos;

namespace SportSpot.V1.WebSockets.Events
{
    public class WebSocketMessageReceivedEvent : IEvent
    {
        public required AbstractWebSocketMessageDto WebSocketMessage { get; init; }
        public required AuthUserEntity Sender { get; init; }
    }
}
