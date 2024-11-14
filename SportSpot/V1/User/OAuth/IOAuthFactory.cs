namespace SportSpot.V1.User.OAuth
{
    public interface IOAuthFactory
    {
        IOAuthProvider GetOAuthProvider(OAuthProviderType provider);
    }
}
