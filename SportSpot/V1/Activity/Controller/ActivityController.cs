using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Activitie.Entities;
using SportSpot.V1.Activity.Dtos;
using SportSpot.V1.Activity.Services;
using SportSpot.V1.Exceptions;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Extensions;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.Activity.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ActivityController(IActivityService _activityService, IUserService _userService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {
        [Authorize]
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityCreateRequestDto request)
        {
            ActivityDto activity = await _activityService.CreateActivity(request, await User.GetAuthUser(_userManager));
            return new ObjectResult(activity)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        [Authorize]
        [HttpGet("{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetActivity(Guid activityId)
        {
            ActivityDto activity = await _activityService.GetDto(activityId, await User.GetAuthUser(_userManager));
            return Ok(activity);
        }


        [Authorize]
        [HttpPut("{activityId}/join")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> JoinActivity(Guid activityId)
        {
            await _activityService.Join(await User.GetAuthUser(_userManager), await _activityService.Get(activityId));
            return Ok();
        }

        [Authorize]
        [HttpPut("{activityId}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> LeaveActivity(Guid activityId)
        {
            await _activityService.Leave(await User.GetAuthUser(_userManager), await _activityService.Get(activityId));
            return Ok();
        }

        [Authorize]
        [HttpPut("{activityId}/{userId}/kick")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> KickUser(Guid activityId, Guid userID)
        {
            AuthUserEntity targetUser = await _userService.GetUser(userID);
            AuthUserEntity currentUser = await User.GetAuthUser(_userManager);
            ActivityEntity activity = await _activityService.Get(activityId);
            await _activityService.KickUser(targetUser, activity, currentUser);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{activityId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActivityDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> KickUser(Guid activityId)
        {
            AuthUserEntity currentUser = await User.GetAuthUser(_userManager);
            ActivityEntity activity = await _activityService.Get(activityId);
            await _activityService.Delete(currentUser, activity);
            return Ok();
        }
    }
}
