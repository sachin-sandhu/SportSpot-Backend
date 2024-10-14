using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User
{
    public record ClubRegisterRequestDto : AuthUserRegisterRequestDto
    {
        [Required]
        public required string Name { get; init; }
        [Required]
        public required string PhoneNumber { get; init; }
        public AddressDto? Address { get; init; }
    }
}
