namespace SportSpot.V1.Exceptions.Session
{
    public class SessionInvalidDataException : AbstractSportSpotException
    {
        public SessionInvalidDataException() : base("Session.InvalidDate", "Session date is invalid.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
