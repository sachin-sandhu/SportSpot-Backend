using SportSpot.V1.Session.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Session.Dtos
{
    public record SessionCreateRequestDto
    {
        [Required]
        public required SportType SportType { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        public required string Title { get; set; }
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public required double Latitude { get; set; }
        [Required]
        public required double Longitude { get; set; }
        [Required]
        public required DateTime Date { get; set; }
        [Required]
        public required int MinParticipants { get; set; }
        [Required]
        public required int MaxParticipants { get; set; }
        public List<string> Tags { get; set; } = [];
    }
}
