namespace SportSpot.V1.Exceptions.Session
{
    public class SessionNotCreatorException : AbstractSportSpotException
    {
        public SessionNotCreatorException() : base("Session.NotCreator", "The user is not the creator of the session.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
