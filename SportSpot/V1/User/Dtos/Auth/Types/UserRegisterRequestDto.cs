namespace SportSpot.V1.User
{
    public record UserRegisterRequestDto : AuthUserRegisterRequestDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}
