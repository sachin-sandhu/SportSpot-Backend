using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportSpot.V1.User.Entities
{
    [Index(nameof(UserId), nameof(Token))]
    public class RefreshTokenEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public required AuthUserEntity User { get; set; }
        public required Guid UserId { get; set; }


        public required string Token { get; set; }
        public required DateTime ExpiryTime { get; set; }
        public required string AccessToken { get; set; }
    }
}
