using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Location.Dtos
{
    public record LocationReverseQueryDto
    {
        [Required]
        public double Latitude { get; init; }
        [Required]
        public double Longitude { get; init; }
    }
}
