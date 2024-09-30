using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User
{
    public record AuthUserRegisterRequestDto
    {
        public required string Username { get; init; }
        [EmailAddress]
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
