using Microsoft.AspNetCore.Identity;

namespace SportSpot.V1.User
{
    public class AuthUserEntity : IdentityUser<Guid>
    {
        public string? AvatarName { get; set; }
        public string Biography { get; set; } = string.Empty;
        public ProfileType ProfileType { get; set; } = ProfileType.NONE;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? AvatarId { get; set; }
        public bool IsOAuth { get; set; }
        public OAuthProviderType OAuthProviderType { get; set; }
    }
}
