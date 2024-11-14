using SportSpot.Events;

namespace SportSpot_Test.Event.Listener
{
    internal class TestEventListener : IListener
    {
        public string Data { get; set; } = string.Empty;
        public int Count { get; set; } = 0;

        [EventHandler]
        public void OnTestEvent(OnTestEvent @event)
        {
            Count += 1;
            Data = @event.Data;
        }
    }
}
