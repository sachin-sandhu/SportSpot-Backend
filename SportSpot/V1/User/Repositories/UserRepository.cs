
using SportSpot.V1.User.Context;

namespace SportSpot.V1.User.Repositories
{
    public class UserRepository(DatabaseContext _context) : IUserRepository
    {
        public async Task<UserEntity> CreateUser(UserEntity user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(Guid id)
        {
            UserEntity? user = await _context.User.FindAsync(id);
            if (user is null) return;
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetUserById(Guid id)
        {
            return await _context.User.FindAsync(id);
        }

        public async Task<UserEntity> UpdateUser(UserEntity user)
        {
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
