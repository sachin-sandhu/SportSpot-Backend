namespace SportSpot.V1.Exceptions.WebSocket
{
    public class UnkownWebSocketMessageException(string rawType) : AbstractSportSpotException("WebSocket.UnkownMessage", $"Unkown Message! Type: {rawType}", StatusCodes.Status400BadRequest)
    {
    }
}
