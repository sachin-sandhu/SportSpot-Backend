namespace SportSpot.V1.Exceptions.WebSocket
{
    public class InvalidWebSocketMessageException : AbstractSportSpotException
    {
        public InvalidWebSocketMessageException() : base("WebSocket.InvalidMessage", "Invalid Message", StatusCodes.Status400BadRequest)
        {
        }
    }
}
