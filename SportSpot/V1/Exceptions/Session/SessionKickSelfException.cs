namespace SportSpot.V1.Exceptions.Session
{
    public class SessionKickSelfException : AbstractSportSpotException
    {
        public SessionKickSelfException() : base("Session.Kick.Self", "You can not kick yourself from the session.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
