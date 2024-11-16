using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Services
{
    public interface IUserService
    {
        Task<AuthUserEntity> GetUser(Guid userId);
        Task<byte[]> GetAvatar(AuthUserEntity user);
    }
}