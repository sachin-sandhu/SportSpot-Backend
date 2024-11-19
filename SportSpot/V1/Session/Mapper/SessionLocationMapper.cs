using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;

namespace SportSpot.V1.Session.Mapper
{
    public static class SessionLocationMapper
    {
        public static SessionLocationDto ToDto(this SessionLocationEntity entity)
        {
            return new SessionLocationDto
            {
                City = entity.City,
                ZipCode = entity.ZipCode
            };
        }
    }
}