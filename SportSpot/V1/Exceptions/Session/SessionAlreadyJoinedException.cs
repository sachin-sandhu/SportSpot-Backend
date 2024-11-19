namespace SportSpot.V1.Exceptions.Session
{
    public class SessionAlreadyJoinedException : AbstractSportSpotException
    {
        public SessionAlreadyJoinedException() : base("Session.AlreadyJoined", "The user has already joined the session.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
