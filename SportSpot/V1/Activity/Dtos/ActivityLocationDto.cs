namespace SportSpot.V1.Activity.Dtos
{
    public record ActivityLocationDto
    {
        public required string City { get; set; }
        public required string ZipCode { get; set; }
    }
}
