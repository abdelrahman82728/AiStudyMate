using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyMate.API.Contracts;
using StudyMate.API.Services;
using StudyMate.API.Models.ModelsAi;

namespace StudyMate.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : Controller
    {
        private readonly AiServices _aiServices;
        public AiController(AiServices aiServices)
        {
            _aiServices = aiServices;

        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PromptHandeling([FromBody] UserPromptDTO clientPromptDto)
        {

            if (!ModelState.IsValid)
            {
                // This will often contain the reason for the 'null' (e.g., failed required field)
                return BadRequest(ModelState);
            }

            int userId = GetAuthenticatedUserId();

           var apiResponse = await _aiServices.HandleUserPromptAsync(userId, clientPromptDto);

            if (!apiResponse.Success)
            {
                return BadRequest(ModelState);
            }

            return Ok(apiResponse.Data);
        }


        [Authorize]
        [HttpGet ("GetConvoShells")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task <ActionResult> GetAllConvoShells()
        {

            int userId = GetAuthenticatedUserId();
            var result = await _aiServices.GetAllConversationByUserId(userId);
            return Ok( result.Data);
        }

        [Authorize]
        [HttpGet("GetAllMessages/{convoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetAllMessages(int convoId)
        {

            int userId = GetAuthenticatedUserId();

            if(convoId < 0)
            {
                return BadRequest("convoId shouldnt  be of Negative value");
            }

            try
            {
                var result = await _aiServices.GetAllMessagesofConvo(convoId, userId);
                return Ok(result.Data);
            }
                 catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            } 
        }
        private int GetAuthenticatedUserId()
        {
            // The 'User' property is inherited from the ControllerBase
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            // This check should always pass if [Authorize] is used
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int authenticatedUserId))
            {
                // Throwing an exception is cleaner than returning an HTTP error here, 
                // as the caller (the controller action) handles the response.
                throw new UnauthorizedAccessException("Authenticated User ID claim is missing.");
            }
            return authenticatedUserId;
        }

    }
}