namespace SportSpot.V1.User.Services
{
    public interface IUserService
    {
        Task<UserEntity> GetUserById(Guid id);
        Task<UserEntity> CreateUser(Guid id, UserRegisterRequestDto request);
        Task DeleteUser(Guid id);
        Task DeleteUser(UserEntity user);
    }
}
