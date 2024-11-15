using Microsoft.AspNetCore.Identity;
using SportSpot.V1.User.OAuth;

namespace SportSpot.V1.User.Entities
{
    public class AuthUserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Biography { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? AvatarId { get; set; }
        public bool IsOAuth { get; set; }
        public OAuthProviderType OAuthProviderType { get; set; }
    }
}
