namespace SportSpot.V1.User
{
    public record JwtConfigurationDto
    {
        public required string Secret { get; init; }
        public required string ValidIsUser { get; init; }
        public required string ValidAudience { get; init; }
        public required TimeSpan Duration { get; init; }
    }
}
