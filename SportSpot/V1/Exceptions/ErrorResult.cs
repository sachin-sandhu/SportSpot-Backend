namespace SportSpot.V1.Exceptions
{
    public record ErrorResult
    {
        public required string Code { get; init; }
        public required string Message { get; init; }
    }
}
