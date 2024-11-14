using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Events
{
    public record AuthUserCreatedEvent : IEvent
    {
        public required AuthUserEntity AuthUserEntity { get; init; }
    }
}
