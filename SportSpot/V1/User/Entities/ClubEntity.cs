namespace SportSpot.V1.User
{
    public class ClubEntity : AuthUserEntity
    {
        public string Name { get; set; } = string.Empty;
        public AddressEntity? Address { get; set; }
    }
}
