namespace SportSpot.V1.User.Dtos
{
    public record UpdateUserDto
    {
        // Auth properties
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
        public required string OldPassword { get; init; }
        public required string NewPassword { get; init; }
    }
}
