using SportSpot.Events.Services;
using SportSpot.Listener;

namespace SportSpot.Events.Extensions
{
    public static class EventExtension
    {
        private readonly static List<Type> _listener = [typeof(TestListener)];

        public static void RegisterEvents(this IServiceCollection col)
        {
            _listener.ForEach(x =>
            {
                if (!x.GetInterfaces().Contains(typeof(IListener))) throw new InvalidOperationException("Listener must implement IListener");
                col.AddSingleton(x);
            });
        }

        public static void RegisterEvents(this IServiceProvider col)
        {
            IEventService eventService = col.GetRequiredService<IEventService>();
            _listener.ForEach(x => eventService.RegisterListener((IListener)col.GetRequiredService(x) ?? throw new InvalidOperationException("Listener not found")));
        }
    }
}
