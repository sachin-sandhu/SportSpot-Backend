using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Session.Chat.Dtos;
using SportSpot.V1.Session.Chat.Services;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.Session.Services;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Extensions;

namespace SportSpot.V1.Session.Chat.Controller
{
    [Route("api/v{version:apiVersion}/Session/{sessionId}/[controller]")]
    [ApiController]
    public class ChatController(IMessageService _messageService, ISessionService _sessionService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {

        [Authorize]
        [HttpGet("messages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MessageDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetMessages([FromRoute] Guid sessionId, [FromQuery] MessageSearchQueryDto searchQuery)
        {
            SessionEntity sessionEntity = await _sessionService.Get(sessionId);
            (List<MessageDto> messages, bool haseMoreEntries) = await _messageService.GetMessages(sessionEntity, searchQuery, await User.GetAuthUser(_userManager));
            Response.Headers.Append("X-Has-More-Entries", haseMoreEntries.ToString());
            return Ok(messages);
        }

    }
}
