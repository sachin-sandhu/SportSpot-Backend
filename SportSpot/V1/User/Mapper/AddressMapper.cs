using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Mapper
{
    public static class AddressMapper
    {

        public static AddressEntity Convert(this AddressDto addressRequestDto)
        {
            return new AddressEntity
            {
                City = addressRequestDto.City,
                Street = addressRequestDto.Street,
                ZipCode = addressRequestDto.ZipCode
            };
        }

        public static AddressDto Convert(this AddressEntity addressEntity)
        {
            return new AddressDto
            {
                City = addressEntity.City,
                Street = addressEntity.Street,
                ZipCode = addressEntity.ZipCode
            };
        }
    }
}
