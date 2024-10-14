using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;

namespace SportSpot.V1.User
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register/club")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> Register([FromBody] ClubRegisterRequestDto request)
        {
            return Ok(await _authService.Register(request));
        }

        [AllowAnonymous]
        [HttpPost("register/user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto request)
        {
            return Ok(await _authService.Register(request));
        }

        [AllowAnonymous]
        [HttpPost("oauth")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> OAuth([FromBody] OAuthUserRequestDto request)
        {
            return Ok(await _authService.OAuth(request));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AuthUserLoginRequestDto request)
        {
            return Ok(await _authService.Login(request));
        }

        [Authorize]
        [HttpDelete("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete()
        {
            await _authService.Delete(await User.GetAuthUser(_userManager));
            return Ok();
        }

        [Authorize]
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            return Ok(await _authService.RefreshAccessToken(await User.GetAuthUser(_userManager), await HttpContext.GetTokenAsync("Bearer", "Access_Token") ?? throw new UnauthorizedException(), request));
        }

        [Authorize]
        [HttpDelete("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Revoke()
        {
            await _authService.RevokeRefreshToken(await User.GetAuthUser(_userManager));
            return Ok();
        }
    }
}
