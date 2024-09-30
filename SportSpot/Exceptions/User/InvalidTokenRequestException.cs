namespace SportSpot.Exceptions.User
{
    public class InvalidTokenRequestException : AbstractSportSpotException
    {
        public InvalidTokenRequestException() : base("Invalid Token Request")
        {
        }
    }
}
