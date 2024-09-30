namespace SportSpot.V1.User
{
    public record AccessTokenDto
    {
        public required string Token { get; init; }
        public required DateTime Expire { get; init; }
    }
}
