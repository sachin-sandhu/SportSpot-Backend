using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot.V1.Location
{
    public class LocationSearchListResponseDto
    {
        public long Count { get; set; }
        public List<LocationSearchResponseDto> Entries { get; set; } = [];
    }
}
