namespace SportSpot.V1.Exceptions.Session
{
    public class SessionFullException : AbstractSportSpotException
    {
        public SessionFullException() : base("Session.Full", "Session is full.", StatusCodes.Status400BadRequest)
        {
        }
    }
}