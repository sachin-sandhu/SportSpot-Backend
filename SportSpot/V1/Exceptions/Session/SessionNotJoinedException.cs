namespace SportSpot.V1.Exceptions.Session
{
    public class SessionNotJoinedException : AbstractSportSpotException
    {
        public SessionNotJoinedException() : base("Session.NotJoined", "The user is not joined to the session.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
