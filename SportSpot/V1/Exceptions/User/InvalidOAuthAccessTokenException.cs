namespace SportSpot.V1.Exceptions.User
{
    public class InvalidOAuthAccessTokenException : AbstractSportSpotException
    {
        public InvalidOAuthAccessTokenException() : base("User.InvalidOAuthAccessToken", "The OAuth AccessToken is Invalid.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
