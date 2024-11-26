namespace SportSpot.V1.Exceptions.WebSocket
{
    public class UnsupportActionWebSocketMessageException : AbstractSportSpotException
    {
        public UnsupportActionWebSocketMessageException() : base("WebSocket.UnsupportetAction", $"Unsupportet Action!", StatusCodes.Status400BadRequest)
        {
        }
    }
}
