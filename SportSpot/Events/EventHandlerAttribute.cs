namespace SportSpot.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventHandlerAttribute(EventPriority priority = EventPriority.MEDIUM) : Attribute
    {
        public EventPriority Priority { get; private set; } = priority;
    }
}
