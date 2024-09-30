namespace SportSpot.V1.User
{
    public class ClubEntity : ProfileEntity
    {
        public string Name { get; set; } = string.Empty;
        public AddressEntity? Address { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
