namespace SportSpot.V1.Exceptions.Session
{
    public class SessionInvalidDistanceException : AbstractSportSpotException
    {
        public SessionInvalidDistanceException() : base("Session.InvalidDistance", "Distance is not valid!", StatusCodes.Status400BadRequest)
        {
        }
    }
}
