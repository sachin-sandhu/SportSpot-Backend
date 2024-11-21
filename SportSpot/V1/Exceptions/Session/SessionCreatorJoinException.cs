namespace SportSpot.V1.Exceptions.Session
{
    public class SessionCreatorJoinException : AbstractSportSpotException
    {
        public SessionCreatorJoinException() : base("Session.Creator.Join", "The Creator can not join the session.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
