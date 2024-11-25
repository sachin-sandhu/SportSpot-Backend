using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.WebSockets.Mapper
{
    public static class WebSocketMessageTypeMapper
    {
        public static Type GetMessageType(this WebSocketMessageType type)
        {
            return type switch
            {
                WebSocketMessageType.MessageSendRequest => typeof(MessageSendRequestDto),
                WebSocketMessageType.MessageSendResponse => typeof(MessageSendResponseDto),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
