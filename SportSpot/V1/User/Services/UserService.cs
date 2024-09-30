
using SportSpot.Exceptions.User;
using SportSpot.V1.User.Repositories;

namespace SportSpot.V1.User.Services
{
    public class UserService(IUserRepository _userRepository) : IUserService
    {
        public async Task<UserEntity> CreateUser(Guid id, UserRegisterRequestDto request)
        {
            UserEntity entity = new()
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            await _userRepository.CreateUser(entity);
            return entity;
        }

        public async Task DeleteUser(Guid id) => await _userRepository.DeleteUser(id);

        public async Task DeleteUser(UserEntity user) => await DeleteUser(user.Id);

        public async Task<UserEntity> GetUserById(Guid id) => await _userRepository.GetUserById(id) ?? throw new UserNotFoundException();
    }
}
