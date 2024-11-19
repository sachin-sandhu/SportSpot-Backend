using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Activity.Dtos
{
    public record ActivityCreateRequestDto
    {

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        public required string Title { get; set; }
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public required long Latitude { get; set; }
        [Required]
        public required long Longitude { get; set; }
        [Required]
        public required DateTime Date { get; set; }
        [Required]
        public required int MinParticipants { get; set; }
        [Required]
        public required int MaxParticipants { get; set; }
        public List<string> Tags { get; set; } = [];
    }
}
