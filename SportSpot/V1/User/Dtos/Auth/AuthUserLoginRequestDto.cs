namespace SportSpot.V1.User
{
    public record AuthUserLoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
