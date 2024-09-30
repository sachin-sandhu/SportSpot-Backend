namespace SportSpot.V1.User
{
    public record AuthTokenDto
    {
        public required string AccessToken { get; init; }
        public required DateTime AccessExpire { get; init; }
        public required string RefreshToken { get; init; }
        public required DateTime RefreshExpire { get; init; }
    }
}
