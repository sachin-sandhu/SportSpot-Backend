using SportSpot.V1.Activity.Dtos;
using SportSpot.V1.Activity.Entities;

namespace SportSpot.V1.Activity.Mapper
{
    public static class ActivityLocationMapper
    {
        public static ActivityLocationDto ToDto(this ActivityLocationEntity entity)
        {
            return new ActivityLocationDto
            {
                City = entity.City,
                ZipCode = entity.ZipCode
            };
        }
    }
}