namespace SportSpot.V1.Session.Dtos
{
    public record SessionLocationDto
    {
        public required string City { get; set; }
        public required string ZipCode { get; set; }
    }
}
