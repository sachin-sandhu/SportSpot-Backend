using SportSpot.Events;

namespace SportSpot_Test.Event
{
    internal class PrioEventListener : IListener
    {
        public List<EventPriority> PrioritiesCallStack { get; private set; } = [];

        [EventHandler(EventPriority.LOW)]
#pragma warning disable IDE0060
        public void OnTestEventLow(OnTestEvent @event)
        {
            PrioritiesCallStack.Add(EventPriority.LOW);
        }

        [EventHandler(EventPriority.HIGH)]
        public void OnTestEventHigh(OnTestEvent @event)
        {
            PrioritiesCallStack.Add(EventPriority.HIGH);
        }

        [EventHandler(EventPriority.MEDIUM)]
        public void OnTestEventMEDIUM(OnTestEvent @event)
        {
            PrioritiesCallStack.Add(EventPriority.MEDIUM);
        }
#pragma warning restore IDE0060
    }
}
