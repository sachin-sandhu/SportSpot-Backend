using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Media.Entities
{
    public record MediaEntity
    {
        [Key]
        public required Guid Id { get; set; }
        public required string FileName { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string FileExtension { get => FileName.Split('.').LastOrDefault(string.Empty); }
        public bool Blocked { get; set; }
        public string? BlurHash { get; set; }
    }
}
