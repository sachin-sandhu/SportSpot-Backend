namespace SportSpot.V1.Location.Dtos
{
    public record LocationConfigDto
    {
        public required string AzureMapsSubscriptionKey { get; init; }
        public required string AzureMapsSearchEndpoint { get; init; }
        public required string AzureMapsReverseLocationEndpoint { get; init; }
    }
}
