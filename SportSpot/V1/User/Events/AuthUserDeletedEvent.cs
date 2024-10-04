namespace SportSpot.V1.User
{
    public record AuthUserDeletedEvent : IEvent
    {
        public required AuthUserEntity AuthUser { get; init; }
    }
}
