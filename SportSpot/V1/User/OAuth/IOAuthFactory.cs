namespace SportSpot.V1.User
{
    public interface IOAuthFactory
    {
        IOAuthProvider GetOAuthProvider(OAuthProviderType provider);
    }
}
