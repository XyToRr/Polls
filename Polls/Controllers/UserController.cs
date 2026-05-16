using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polls.Business.Interfaces;
using System.Security.Claims;

namespace Polls.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("{targetUserId}")]
        [Authorize]
        public async Task<IActionResult> AddUserFollow(Guid targetUserId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized();
            }

            if (targetUserId == Guid.Empty)
            {
                return BadRequest("Target user ID cannot be empty");
            }

            try
            {
                var result = await _userService.AddFollowAsync(currentUserId, targetUserId);
                if (!result)
                {
                    return BadRequest("Cannot follow user. User may not exist or already following.");
                }

                return CreatedAtAction(nameof(AddUserFollow), null);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{targetUserId}")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFollow(Guid targetUserId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim) || !Guid.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized();
            }

            if (targetUserId == Guid.Empty)
            {
                return BadRequest("Target user ID cannot be empty");
            }

            try
            {
                var result = await _userService.RemoveFollowAsync(currentUserId, targetUserId);
                if (!result)
                {
                    return NotFound("Follow relationship not found.");
                }

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

