using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;
using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Extensions;
using SportSpot.V1.User.Mapper;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.User.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController(IUserService _userService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {

        [Authorize]
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            return Ok((await _userService.GetUser(userId)).ConvertToDto(await User.GetAuthUser(_userManager)));
        }

        [HttpGet("{userId}/avatar")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK, "image/png")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetAvatar(Guid userId)
        {
            return File(await _userService.GetAvatar(await _userService.GetUser(userId)), "image/png");
        }

        [Authorize]
        [HttpPatch("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            AuthUserEntity authUser = await User.GetAuthUser(_userManager);
            await _userService.Update(updateUserDto, authUser);
            return Ok();
        }
    }
}
