using SportSpot.V1.Session.Enums;
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
        public int Size { get; init; } = 10;
        
        [DefaultValue(0)]
        public int Page { get; init; } = 0;

        public SportType? SportType { get; init; }
    }
}
