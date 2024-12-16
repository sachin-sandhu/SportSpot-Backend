using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User.Dtos
{
    public record UpdateUserDto
    {
        // Auth properties
        [EmailAddress]
        public string? Email { get; init; }
        public string? Username { get; init; }
        public ChangePasswordDto? Password { get; init; }

        // User properties
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Biography { get; set; }
        public string? AvatarAsBase64 { get; init; }
    }

    public record ChangePasswordDto
    {
        [Required]
        public required string OldPassword { get; init; }
        [Required]
        public required string NewPassword { get; init; }
    }
}
