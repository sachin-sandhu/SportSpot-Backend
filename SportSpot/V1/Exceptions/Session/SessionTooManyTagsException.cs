namespace SportSpot.V1.Exceptions.Session
{
    public class SessionTooManyTagsException : AbstractSportSpotException
    {
        public SessionTooManyTagsException() : base("Session.TooManyTags", "Too many tags!", StatusCodes.Status400BadRequest)
        {
        }
    }
}
