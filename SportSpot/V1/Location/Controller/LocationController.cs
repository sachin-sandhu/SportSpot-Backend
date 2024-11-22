using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Services;

namespace SportSpot.V1.Location.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LocationController(ILocationService _locationService) : ControllerBase
    {

        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> SearchLocation([FromQuery] LocationSearchQueryDto query)
        {
            List<LocationDto> locations = await _locationService.GetLocations(query.SearchQuery, "DE", "de-DE", query.EntityType);
            return Ok(locations);
        }

        [Authorize]
        [HttpGet("reverse")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AzureAddressDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> ReverseLocation([FromQuery] LocationReverseQueryDto query)
        {
            AzureAddressDto address = await _locationService.GetAddress("de-DE", query.Latitude, query.Longitude);
            return Ok(address);
        }

    }
}
