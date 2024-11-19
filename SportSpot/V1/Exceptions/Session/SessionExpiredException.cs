namespace SportSpot.V1.Exceptions.Session
{
    public class SessionExpiredException : AbstractSportSpotException
    {
        public SessionExpiredException() : base("Session.Expired", "Session is expired.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
