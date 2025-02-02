using SportSpot.V1.Exceptions.WebSocket;
using SportSpot.V1.WebSockets.Dtos;
using SportSpot.V1.WebSockets.Enums;
using SportSpot.V1.WebSockets.Mapper;
using System.Text.Json.Serialization;

namespace SportSpot.V1.WebSockets.Converter
{
    public class WebSocketMessageConverter : JsonConverter<IWebSocketMessageDto>
    {
        public override IWebSocketMessageDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            string? rawType = GetProperty(doc.RootElement, "MessageType");
            rawType ??= GetProperty(doc.RootElement, "messageType");
            if (rawType == null)
                throw new InvalidWebSocketMessageException();
            if (!Enum.TryParse(rawType, out WebSocketMessageType type))
                throw new UnkownWebSocketMessageException(rawType);
            object obj = JsonSerializer.Deserialize(doc.RootElement.GetRawText(), type.GetMessageType(), options) ?? throw new InvalidWebSocketMessageException();
            return (IWebSocketMessageDto)obj;
        }

        public override void Write(Utf8JsonWriter writer, IWebSocketMessageDto value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.MessageType.GetMessageType(), options);
        }

        private static string? GetProperty(JsonElement element, string key)
        {
            return element.TryGetProperty(key, out JsonElement value) ? value.GetString() : null;
        }
    }
}
