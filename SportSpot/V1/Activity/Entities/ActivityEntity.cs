using SportSpot.V1.Activity.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportSpot.V1.Activitie.Entities
{
    public record ActivityEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }
        public required Guid CreatorId { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required ActivityLocationEntity Location { get; set; }
        public required DateTime Date { get; set; }
        public required int MinParticipants { get; set; }
        public required int MaxParticipants { get; set; }

        public List<string> Tags { get; set; } = [];
        public List<Guid> Participants { get; set; } = [];
    }
}
