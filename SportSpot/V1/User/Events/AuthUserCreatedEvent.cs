namespace SportSpot.V1.User
{
    public record AuthUserCreatedEvent : IEvent
    {
        public required AuthUserEntity AuthUserEntity { get; init; }
    }
}
