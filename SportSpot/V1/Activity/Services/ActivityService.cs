using SportSpot.Events.Services;
using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.Activity.Dtos;
using SportSpot.V1.Activity.Entities;
using SportSpot.V1.Activity.Events;
using SportSpot.V1.Activity.Mapper;
using SportSpot.V1.Activity.Repositories;
using SportSpot.V1.Exceptions.Activity;
using SportSpot.V1.Exceptions.Location;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Services;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Activity.Services
{
    public class ActivityService(IActivityRepository _activityRepository, ILocationService _locationService, IEventService _eventService) : IActivityService
    {
        public async Task<ActivityDto> CreateActivity(ActivityCreateRequestDto createRequestDto, AuthUserEntity user)
        {
            AzureAddressDto adress = await _locationService.GetAddress("de-DE", createRequestDto.Latitude, createRequestDto.Longitude);
            if (adress.Municipality == null || adress.PostalCode == null)
            {
                throw new LocationNotFoundException();
            }

            if (createRequestDto.Tags.Count > 50)
            {
                throw new ActivityTooManyTagsException();
            }

            foreach (string tag in createRequestDto.Tags)
            {
                if (tag.Length > 50)
                {
                    throw new ActivityTagTooLongException();
                }
            }

            if (DateTime.Now >= createRequestDto.Date)
                throw new ActivityInvalidDataException();

            ActivityEntity activityEntity = new()
            {
                Title = createRequestDto.Title,
                Description = createRequestDto.Description,
                Date = createRequestDto.Date,
                CreatorId = user.Id,
                CreatedAt = DateTime.Now,
                MinParticipants = createRequestDto.MinParticipants,
                MaxParticipants = createRequestDto.MaxParticipants,
                Location = new ActivityLocationEntity
                {
                    Latitude = createRequestDto.Latitude,
                    Longitude = createRequestDto.Longitude,
                    City = adress.Municipality,
                    ZipCode = adress.PostalCode
                },
                Tags = createRequestDto.Tags,
            };

            await _activityRepository.Add(activityEntity);
            return activityEntity.ToDto();
        }

        public async Task<ActivityEntity> Get(Guid activityId)
        {
            return await _activityRepository.GetActivityAsync(activityId) ?? throw new ActivityNotFoundException();
        }

        public async Task<ActivityDto> GetDto(Guid activityId, AuthUserEntity user)
        {
            ActivityEntity activity = await Get(activityId);
            return activity.ToDto(IsMember(user, activity));
        }

        public async Task Join(AuthUserEntity user, ActivityEntity activity)
        {
            if (activity.CreatorId == user.Id)
            {
                throw new ActivityCreatorJoinException();
            }
            if (activity.Participants.Contains(user.Id))
            {
                throw new ActivityAlreadyJoinedException();
            }
            if (activity.Participants.Count + 1 >= activity.MaxParticipants)
            {
                throw new ActivityFullException();
            }
            if (DateTime.Now >= activity.Date)
            {
                throw new ActivityExpiredException();
            }
            activity.Participants.Add(user.Id);
            await _activityRepository.UpdateActitiy(activity);
        }

        public async Task KickUser(AuthUserEntity target, ActivityEntity activity, AuthUserEntity sender)
        {
            if (target.Id == sender.Id)
            {
                throw new ActivityKickSelfException();
            }
            if (activity.CreatorId != sender.Id)
            {
                throw new ActivityNotCreatorException();
            }
            if (!activity.Participants.Contains(target.Id))
            {
                throw new ActivityNotJoinedException();
            }
            activity.Participants.Remove(target.Id);
            await _activityRepository.UpdateActitiy(activity);
        }

        public async Task Leave(AuthUserEntity user, ActivityEntity activity)
        {
            if (activity.CreatorId == user.Id)
            {
                throw new ActivityCreatorLeaveException();
            }
            if (!activity.Participants.Contains(user.Id))
            {
                throw new ActivityNotJoinedException();
            }
            activity.Participants.Remove(user.Id);
            await _activityRepository.UpdateActitiy(activity);
        }
        public async Task Delete(AuthUserEntity user, ActivityEntity activity)
        {
            if (user.Id != activity.CreatorId)
            {
                throw new ActivityNotCreatorException();
            }
            await _activityRepository.DeleteActitiy(activity);
            await _eventService.FireEvent(new ActivityDeletedEvent { ActivityEntity = activity, Executer = user });
        }

        private static bool IsMember(AuthUserEntity authUserEntity, ActivityEntity activity) => activity.CreatorId == authUserEntity.Id || activity.Participants.Contains(authUserEntity.Id);

    }
}
