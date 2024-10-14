namespace SportSpot.V1.Exceptions
{
    public class InvalidTokenRequestException : AbstractSportSpotException
    {
        public InvalidTokenRequestException() : base("User.InvalidToken", "Invalid Token Request", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
