namespace SportSpot.V1.Exceptions
{
    public class OAuthProviderException : AbstractSportSpotException
    {
        public OAuthProviderException() : base("User.WrongOAuthProvider", "The account has a different OAuth provider!", StatusCodes.Status400BadRequest)
        { }
    }
}
