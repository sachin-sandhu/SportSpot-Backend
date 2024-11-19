using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Activity.Events
{
    public class ActivityDeletedEvent : IEvent
    {
        public required ActivityEntity ActivityEntity { get; set; }
        public required AuthUserEntity Executer { get; set; }
    }
}
