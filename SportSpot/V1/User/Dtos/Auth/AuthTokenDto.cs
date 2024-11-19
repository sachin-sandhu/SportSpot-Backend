namespace SportSpot.V1.User.Dtos.Auth
{
    public record AuthTokenDto
    {
        public required Guid UserId { get; init; }
        public required string AccessToken { get; init; }
        public required DateTime AccessExpire { get; init; }
        public required string RefreshToken { get; init; }
        public required DateTime RefreshExpire { get; init; }
    }
}
