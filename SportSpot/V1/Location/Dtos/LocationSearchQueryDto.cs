using SportSpot.V1.Location.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SportSpot.V1.Location.Dtos
{
    public record LocationSearchQueryDto
    {
        [Required]
        public required string SearchQuery { get; set; }
        [DefaultValue(AzureGeographicEntityType.All)]
        public AzureGeographicEntityType EntityType { get; set; }
    }
}
