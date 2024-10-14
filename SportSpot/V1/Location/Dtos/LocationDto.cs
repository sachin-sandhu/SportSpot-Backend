using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot.V1.Location
{
    public class LocationDto
    {
        public required AzureAddressDto Address { get; set; }
        public required AzurePositionDto Position { get; set; }
    }
}
