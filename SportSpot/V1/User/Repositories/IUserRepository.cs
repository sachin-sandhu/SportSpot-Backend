namespace SportSpot.V1.User
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserById(Guid id);
        Task<UserEntity> CreateUser(UserEntity user);
        Task<UserEntity> UpdateUser(UserEntity user);
        Task DeleteUser(Guid id);
    }
}
