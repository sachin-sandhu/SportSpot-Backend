using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.Activity.Dtos;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Activity.Services
{
    public interface IActivityService
    {
        Task<ActivityDto> CreateActivity(ActivityCreateRequestDto createRequestDto, AuthUserEntity user);
        Task KickUser(AuthUserEntity target, ActivityEntity activity, AuthUserEntity sender);
        Task Leave(AuthUserEntity user, ActivityEntity activity);
        Task Join(AuthUserEntity user, ActivityEntity activity);
        Task Delete(AuthUserEntity user, ActivityEntity activity);
        Task<ActivityDto> GetDto(Guid activityId, AuthUserEntity user);
        Task<ActivityEntity> Get(Guid activityId);
    }
}
