namespace SportSpot.V1.User
{
    public record RefreshTokenRequestDto
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
    }
}
