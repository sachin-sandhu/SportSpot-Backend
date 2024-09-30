namespace SportSpot.V1.User
{
    public record AddressDto
    {
        public required string City { get; init; }
        public required string Street { get; init; }
        public required string ZipCode { get; init; }
    }
}
