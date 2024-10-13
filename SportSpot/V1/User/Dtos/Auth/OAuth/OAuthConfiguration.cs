namespace SportSpot.V1.User
{
    public record OAuthConfiguration
    {
        public required string GoogleUserInformationEndpoint { get; init; }
    }
}
