namespace SportSpot.V1.Exceptions.Session
{
    public class SessionCreatorLeaveException : AbstractSportSpotException
    {
        public SessionCreatorLeaveException() : base("Session.CreatorLeave", "The creator of the session cannot leave it.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
