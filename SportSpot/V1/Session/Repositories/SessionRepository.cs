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

        public async Task<List<SessionEntity>> GetSessionsInRange(int size, int page, double lat, double lng, int maxDistance, Guid userID)
        {
            List<SessionEntity> entries = await _context.Session
                .Where(x => (
                x.CreatorId == userID || x.Participants.Contains(userID)) 
                && x.Date > DateTime.Now 
                && CalcDistance(lat, lng, x.Location.Latitude, x.Location.Longitude) <= maxDistance)
                .Skip(page * size)
                .Take(size).ToListAsync();
            return entries;
        }

        private static double CalcDistance(double lat1, double lng1, double lat2, double lng2) 
        {
            const double R = 6372.8; // In kilometers
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lng2 - lng1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }
}
