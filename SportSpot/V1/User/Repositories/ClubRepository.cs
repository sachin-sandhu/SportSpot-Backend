namespace SportSpot.V1.User
{
    public class ClubRepository(DatabaseContext _context) : IClubRepositoriy
    {
        public async Task<ClubEntity> CreateClub(ClubEntity club)
        {
            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
            return club;
        }

        public async Task DeleteClub(Guid id)
        {
            ClubEntity? entity = await _context.Clubs.FindAsync(id);
            if (entity is null) return;
            _context.Clubs.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<ClubEntity?> GetClubById(Guid id)
        {
            return await _context.Clubs.FindAsync(id);
        }

        public async Task<ClubEntity> UpdateClub(ClubEntity club)
        {
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
            return club;
        }
    }
}
