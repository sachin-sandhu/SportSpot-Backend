using SportSpot.V1.Request;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.OAuth.Types;

namespace SportSpot.V1.User.OAuth
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
