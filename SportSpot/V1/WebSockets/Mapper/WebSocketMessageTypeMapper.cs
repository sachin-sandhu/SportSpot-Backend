using SportSpot.V1.WebSockets.Enums;

namespace SportSpot.V1.WebSockets.Mapper
{
    public static class WebSocketMessageTypeMapper
    {
        public static Type GetMessageType(this WebSocketMessageType type)
        {
            return type switch
            {
                _ => throw new NotImplementedException(),
            };
        }
    }
}
