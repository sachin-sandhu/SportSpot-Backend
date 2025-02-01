using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Dtos.Auth.OAuth;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Extensions;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.User.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> Register([FromBody] AuthUserRegisterRequestDto request)
        {
            return new ObjectResult(await _authService.Register(request))
            {
                StatusCode = StatusCodes.Status201Created
            };
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

        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthTokenDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            return Ok(await _authService.RefreshAccessToken(request));
        }

        [Authorize]
        [HttpDelete("token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Revoke()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token") ?? throw new UnauthorizedException();
            await _authService.RevokeRefreshToken(await User.GetAuthUser(_userManager), accessToken);
            return Ok();
        }
    }
}
