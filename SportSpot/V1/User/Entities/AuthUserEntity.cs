using Microsoft.AspNetCore.Identity;

namespace SportSpot.V1.User
{
    public class AuthUserEntity : IdentityUser<Guid>
    {
        public ProfileType ProfileType { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
