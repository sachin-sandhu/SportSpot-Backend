namespace SportSpot.V1.Activity.Entities
{
    public record ActivityLocationEntity
    {
        public required long Latitude { get; set; }
        public required long Longitude { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }
    }
}
