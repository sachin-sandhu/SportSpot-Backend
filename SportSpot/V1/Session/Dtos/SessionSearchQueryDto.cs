using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Session.Dtos
{
    public record SessionSearchQueryDto
    {
        [Required]
        public required int Distance { get; init; }

        [Required]
        public double Latitude { get; init; }

        [Required]
        public double Longitude { get; init; }

        [DefaultValue(10)]
        public int Size { get; init; }
        
        [DefaultValue(0)]
        public int Page { get; init; }
    }
}
