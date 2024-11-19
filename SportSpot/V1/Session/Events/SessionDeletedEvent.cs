using SportSpot.V1.Session.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Session.Events
{
    public class SessionDeletedEvent : IEvent
    {
        public required SessionEntity SessionEntity { get; set; }
        public required AuthUserEntity Executer { get; set; }
    }
}
