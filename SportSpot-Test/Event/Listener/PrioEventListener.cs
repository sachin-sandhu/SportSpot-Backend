using SportSpot.Events;

namespace SportSpot_Test.Event.Listener
{
    internal class PrioEventListener : IListener
    {
        public List<EventPriority> PrioritiesCallStack { get; private set; } = [];

        [EventHandler(EventPriority.LOW)]
        public void OnTestEventLow(OnTestEvent _)
        {
            PrioritiesCallStack.Add(EventPriority.LOW);
        }

        [EventHandler(EventPriority.HIGH)]
        public void OnTestEventHigh(OnTestEvent _)
        {
            PrioritiesCallStack.Add(EventPriority.HIGH);
        }

        [EventHandler(EventPriority.MEDIUM)]
        public void OnTestEventMEDIUM(OnTestEvent _)
        {
            PrioritiesCallStack.Add(EventPriority.MEDIUM);
        }
    }
}
