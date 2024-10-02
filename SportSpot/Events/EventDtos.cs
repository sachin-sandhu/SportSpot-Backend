using System.Reflection;

namespace SportSpot.Events
{
    public record RegisteredListener
    {
        public required IListener Listener { get; init; }
        public Dictionary<Type, List<RegisteredEventHandler>> Events { get; private set; } = [];
    }

    public record RegisteredEventHandler
    {
        public required EventHandlerAttribute Attribute { get; init; }
        public required MethodInfo Method { get; init; }
        public required IListener Listener { get; init; }
    }
}
