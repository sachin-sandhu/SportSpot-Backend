namespace SportSpot.V1.Location
{
    public class AzureReverseLocationResultDto
    {
        public required string Id { get; set; }
        public required AzureAddressDto Address { get; set; }
        public required string Position { get; set; }
    }
}
