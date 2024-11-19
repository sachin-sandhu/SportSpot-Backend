namespace SportSpot.V1.Session.Entities
{
    public record SessionLocationEntity
    {
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }
    }
}
