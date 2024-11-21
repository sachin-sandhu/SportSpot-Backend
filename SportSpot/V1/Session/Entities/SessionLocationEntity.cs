using MongoDB.Driver.GeoJsonObjectModel;

namespace SportSpot.V1.Session.Entities
{
    public record SessionLocationEntity
    {
        public required GeoJson2DGeographicCoordinates Coordinates { get; set; }
        public required string City { get; set; }
        public required string ZipCode { get; set; }
    }
}
