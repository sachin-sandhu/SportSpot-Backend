using SportSpot.V1.Activitie.Entities;

namespace SportSpot.V1.Activity.Repositories
{
    public interface IActivityRepository
    {
        public Task<ActivityEntity> Add(ActivityEntity activityEntity);
        public Task<ActivityEntity?> GetActivityAsync(Guid activityId);
        public Task DeleteActitiy(ActivityEntity activityEntity);
        public Task UpdateActitiy(ActivityEntity activityEntity);
    }
}
