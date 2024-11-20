namespace SportSpot.V1.Exceptions.Session
{
    public class SessionInvalidLocationException : AbstractSportSpotException
    {
        public SessionInvalidLocationException() : base("Session.InvalidLocation", "Invalid Location. Latitude must be between -90 and 90, Longitude must be between -180 and 180.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
