using AzureLocation_Emulator.Dtos;
using AzureLocation_Emulator.Enums;
using AzureLocation_Emulator.Factories;
using AzureLocation_Emulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureLocation_Emulator.Controllers
{
    [ApiController]
    [Route("")]
    public class SearchController(IModeFactory _factory) : ControllerBase
    {
        [HttpGet("/search")]
        public IActionResult Search()
        {
            return _factory.GetModeService(ModeType.Search).GetResult();
        }

        [HttpGet("/reverse")]
        public IActionResult Reverse()
        {
            return _factory.GetModeService(ModeType.Reverse).GetResult();
        }
    }
}
