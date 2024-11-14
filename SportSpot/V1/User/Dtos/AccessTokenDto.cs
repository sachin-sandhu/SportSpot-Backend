namespace SportSpot.V1.User.Dtos
{
    public record AccessTokenDto
    {
        public required string Token { get; init; }
        public required DateTime Expire { get; init; }
    }
}
