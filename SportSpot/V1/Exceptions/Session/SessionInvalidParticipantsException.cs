namespace SportSpot.V1.Exceptions.Session
{
    public class SessionInvalidParticipantsException : AbstractSportSpotException
    {
        public SessionInvalidParticipantsException() : base("Session.TagTooLong", "Invalid Participants Max or Min. (Max > 0 & Min > 0) & Max >= Min", StatusCodes.Status400BadRequest)
        {
        }
    }
}
