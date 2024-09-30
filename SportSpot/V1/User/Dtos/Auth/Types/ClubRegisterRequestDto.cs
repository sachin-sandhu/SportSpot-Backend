namespace SportSpot.V1.User
{
    public record ClubRegisterRequestDto : AuthUserRegisterRequestDto
    {
        public required string Name { get; init; }
        public required string PhoneNumber { get; init; }
        public AddressDto? Address { get; init; }
    }
}
