using Microsoft.EntityFrameworkCore;
using SportSpot.V1.Context;
using SportSpot.V1.Media.Entities;

namespace SportSpot.V1.Media.Repositories
{
    public class MediaRepository(DatabaseContext _context) : IMediaRepository
    {
        public async Task Add(MediaEntity medium)
        {
            _context.Media.Add(medium);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(MediaEntity medium)
        {
            _context.Media.Remove(medium);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MediaEntity medium)
        {
            _context.Media.Update(medium);
            await _context.SaveChangesAsync();
        }

        public async Task<MediaEntity?> Get(Guid id)
        {
            return await _context.Media.FindAsync(id);
        }

        public Task<List<MediaEntity>> GetAll()
        {
            return _context.Media.ToListAsync();
        }
    }
}
