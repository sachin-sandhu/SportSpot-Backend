namespace SportSpot.V1.User
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
