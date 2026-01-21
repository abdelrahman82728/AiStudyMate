using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyMate.API.Services;

namespace StudyMate.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly ProgressServices _progressService;

        public ProgressController(ProgressServices progressService)
        {
            _progressService = progressService;
        }

        // POST: api/Progress/heartbeat
        [HttpPost("heartbeat")]
        public async Task<IActionResult> Heartbeat()
        {
            int userId = GetAuthenticatedUserId();
            await _progressService.TrackHeartbeatAsync(userId);
            return Ok();
        }

        // GET: api/Progress/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            int userId = GetAuthenticatedUserId();
            var stats = await _progressService.GetWeeklyStatsAsync(userId);

            // Map to a DTO for the chart here if needed
            return Ok(stats);
        }

        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authenticatedUserId))
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            return authenticatedUserId;
        }
    }
}