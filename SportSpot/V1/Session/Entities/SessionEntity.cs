using SportSpot.V1.Session.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportSpot.V1.Session.Entities
{
    public record SessionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required SportType SportType { get; set; }
        public required Guid CreatorId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required SessionLocationEntity Location { get; set; }
        public required DateTime Date { get; set; }
        public required int MinParticipants { get; set; }
        public required int MaxParticipants { get; set; }

        public List<string> Tags { get; set; } = [];
        public List<Guid> Participants { get; set; } = [];
    }
}
