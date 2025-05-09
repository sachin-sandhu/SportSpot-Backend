﻿namespace SportSpot.V1.Location.Dtos
{
    public class LocationSearchResponseDto
    {
        public required AzureAddressDto Address { get; set; }
        public required AzurePositionDto Position { get; set; }
    }
}
