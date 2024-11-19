using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.Activity.Dtos;

namespace SportSpot.V1.Activity.Mapper
{
    public static class ActivityMapper
    {
        public static ActivityDto ToDto(this ActivityEntity activity, bool withUser = true)
        {
            ActivityDto activityDto = new()
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Date = activity.Date,
                Location = activity.Location.ToDto(),
                CreatorId = activity.CreatorId,
                MinParticipants = activity.MinParticipants,
                MaxParticipants = activity.MaxParticipants,
                ParticipantsCount = activity.Participants.Count + 1,
                Tags = activity.Tags
            };
            if (withUser)
                activityDto.Participants = activity.Participants;
            return activityDto;
        }
    }
}
