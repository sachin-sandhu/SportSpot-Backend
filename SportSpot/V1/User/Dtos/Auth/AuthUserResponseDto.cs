using System.Text.Json.Serialization;

namespace SportSpot.V1.User.Dtos.Auth
{
    public record AuthUserResponseDto
    {
        public required Guid Id { get; init; }
        public string? Email { get; set; }
        public required string Username { get; init; }
        public required string Biography { get; init; }
        public required string Avatar { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public DateTime? BirthDate { get; set; }
    }
}
