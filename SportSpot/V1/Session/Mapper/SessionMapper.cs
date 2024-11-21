using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;

namespace SportSpot.V1.Session.Mapper
{
    public static class SessionMapper
    {
        public static SessionDto ToDto(this SessionEntity session, bool withUser = true)
        {
            SessionDto sessionDto = new()
            {
                Id = session.Id,
                SportType = session.SportType,
                Title = session.Title,
                Description = session.Description,
                Date = session.Date,
                Location = session.Location.ToDto(),
                CreatorId = session.CreatorId,
                MinParticipants = session.MinParticipants,
                MaxParticipants = session.MaxParticipants,
                ParticipantsCount = session.Participants.Count + 1,
                Tags = session.Tags
            };
            if (withUser)
            {
                sessionDto.Participants = session.Participants;
                sessionDto.Participants.Add(session.CreatorId);
            }
            return sessionDto;
        }
    }
}
