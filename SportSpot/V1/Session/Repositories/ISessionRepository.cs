using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Session.Repositories
{
    public interface ISessionRepository
    {
        public Task Add(SessionEntity sessionEntity);
        public Task<SessionEntity?> GetSession(Guid sessionId);
        public Task DeleteSession(SessionEntity sessionEntity);
        public Task UpdateSession(SessionEntity sessionEntity);
        public Task<List<SessionEntity>> GetAll();
        public Task<(List<SessionEntity>, bool)> GetSessionsInRange(SessionSearchQueryDto requestDto, Guid userID);
        Task<List<SessionEntity>> GetSessionsFromUser(AuthUserEntity user);
    }
}
