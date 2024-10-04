using SportSpot.Events;

namespace SportSpot_Test.Event.Listener
{
    internal class AsyncEventListener : IListener
    {
        public string Data { get; set; } = string.Empty;

        [EventHandler]
        public async Task OnTestEvent(OnTestEvent @event)
        {
            await Task.Delay(10);
            Data = @event.Data;
        }
    }
}
