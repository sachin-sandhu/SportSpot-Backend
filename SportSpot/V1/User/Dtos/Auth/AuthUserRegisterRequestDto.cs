using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User.Dtos.Auth
{
    public record AuthUserRegisterRequestDto
    {
        [Required]
        public required string Username { get; init; }
        [EmailAddress]
        [Required]
        public required string Email { get; init; }
        [Required]
        public required string Password { get; init; }
        [Required]
        public required string FirstName { get; init; }
        [Required]
        public required string LastName { get; init; }
        public string? AvatarAsBase64 { get; init; }
    }
}
