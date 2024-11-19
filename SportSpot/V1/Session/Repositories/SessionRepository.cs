using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Context;
using SportSpot.V1.Session.Entities;

namespace SportSpot.V1.Session.Repositories
{
    public class SessionRepository(DatabaseContext _context) : ISessionRepository
    {
        public async Task<SessionEntity> Add(SessionEntity sessionEntity)
        {
            await _context.Session.AddAsync(sessionEntity);
            await _context.SaveChangesAsync();
            return sessionEntity;
        }

        public async Task DeleteSession(SessionEntity sessionEntity)
        {
            _context.Session.Remove(sessionEntity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSession(SessionEntity sessionEntity)
        {
            _context.Session.Update(sessionEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<SessionEntity?> GetSession(Guid sessionId)
        {
            return await _context.Session.FindAsync(sessionId);
        }

        public async Task<List<SessionEntity>> GetAll()
        {
            return await _context.Session.ToListAsync();
        }
    }
}
