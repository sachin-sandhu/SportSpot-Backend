namespace SportSpot.V1.User.Dtos.Auth
{
    public class OAuthUserDataDto
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public byte[]? Picture { get; set; }
    }
}
