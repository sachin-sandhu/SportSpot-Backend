namespace SportSpot.V1.User
{
    public record OAuthConfigurationDto
    {
        public required string GoogleUserInformationEndpoint { get; init; }
    }
}
