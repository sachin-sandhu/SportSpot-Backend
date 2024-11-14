namespace SportSpot.V1.User.Dtos.Auth
{
    public record RefreshTokenRequestDto
    {
        public required string RefreshToken { get; init; }
    }
}
