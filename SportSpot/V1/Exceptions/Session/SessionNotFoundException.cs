namespace SportSpot.V1.Exceptions.Session
{
    public class SessionNotFoundException : AbstractSportSpotException
    {
        public SessionNotFoundException() : base("Session.NotFound", "Session not found.", StatusCodes.Status404NotFound)
        {
        }
    }
}
