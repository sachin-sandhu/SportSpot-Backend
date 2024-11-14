namespace SportSpot.V1.User.Dtos.Auth.OAuth
{
    public record OAuthConfigurationDto
    {
        public required string GoogleUserInformationEndpoint { get; init; }
    }
}
