using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User
{
    public record UserRegisterRequestDto : AuthUserRegisterRequestDto
    {
        [Required]
        public required string FirstName { get; init; }
        [Required]
        public required string LastName { get; init; }
    }
}
