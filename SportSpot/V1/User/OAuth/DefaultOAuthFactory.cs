
using SportSpot.V1.Request;

namespace SportSpot.V1.User
{
    public class DefaultOAuthFactory(IRequest _request, OAuthConfigurationDto _config) : IOAuthFactory
    {
        public IOAuthProvider GetOAuthProvider(OAuthProviderType provider)
        {
            return provider switch
            {
                OAuthProviderType.GOOGLE => new GoogleOAuthProvider(_config, _request),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
