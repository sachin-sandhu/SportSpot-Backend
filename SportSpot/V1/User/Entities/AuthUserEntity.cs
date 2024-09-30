using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace SportSpot.V1.User
{
    [CollectionName("AuthUser")]
    public class AuthUserEntity : MongoIdentityUser<Guid>
    {
        public ProfileType ProfileType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
