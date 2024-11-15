using Rest_Emulator.Dtos;
using Rest_Emulator.Enums;
using Rest_Emulator.Factories;
using Rest_Emulator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Rest_Emulator.Controllers
{
    [ApiController]
    [Route("")]
    public class SearchController(IModeFactory _factory) : ControllerBase
    {
        [HttpGet("/search")]
        public IActionResult Search()
        {
            return _factory.GetModeService(ModeType.SearchLocation).GetResult();
        }

        [HttpGet("/reverse")]
        public IActionResult Reverse()
        {
            return _factory.GetModeService(ModeType.ReverseLocation).GetResult();
        }
    }
}
