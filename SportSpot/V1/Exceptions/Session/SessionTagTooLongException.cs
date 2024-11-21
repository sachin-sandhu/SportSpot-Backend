namespace SportSpot.V1.Exceptions.Session
{
    public class SessionTagTooLongException : AbstractSportSpotException
    {
        public SessionTagTooLongException() : base("Session.TagTooLong", "Session Tag is too long.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
