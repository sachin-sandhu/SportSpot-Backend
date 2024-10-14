namespace SportSpot.V1.Location
{
    public class AzureLocationResultDto
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public double? Score { get; set; }
        public AzureAddressDto? Address { get; set; }
        public AzurePositionDto? Position { get; set; }
    }
}
