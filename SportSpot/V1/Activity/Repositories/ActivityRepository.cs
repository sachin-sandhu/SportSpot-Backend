using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.Context;

namespace SportSpot.V1.Activity.Repositories
{
    public class ActivityRepository(DatabaseContext _context) : IActivityRepository
    {
        public async Task<ActivityEntity> Add(ActivityEntity activityEntity)
        {
            await _context.Activity.AddAsync(activityEntity);
            await _context.SaveChangesAsync();
            return activityEntity;
        }

        public async Task DeleteActitiy(ActivityEntity activityEntity)
        {
            _context.Activity.Remove(activityEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateActitiy(ActivityEntity activityEntity)
        {
            _context.Activity.Update(activityEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<ActivityEntity?> GetActivityAsync(Guid activityId)
        {
            return await _context.Activity.FindAsync(activityId);
        }
    }
}
