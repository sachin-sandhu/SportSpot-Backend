using SportSpot.Events;

namespace SportSpot_Test.Event
{
    internal record OnTestEvent : IEvent
    {
        public required string Data { get; init; }
    }
}
