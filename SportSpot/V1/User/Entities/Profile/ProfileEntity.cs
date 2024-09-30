using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.User
{
    public class ProfileEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? AvatarName { get; set; }
        public string Biography { get; set; } = string.Empty;
    }
}