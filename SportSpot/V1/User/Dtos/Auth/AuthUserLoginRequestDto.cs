namespace SportSpot.V1.User.Dtos.Auth
{
    public record AuthUserLoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
