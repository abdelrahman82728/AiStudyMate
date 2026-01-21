using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using StudyMate.API.Models.ModelsAuth;
using StudyMate.API.Models.ModelsNotes;
using StudyMate.API.Models.ModelsNotes;
using StudyMate.API.Services;

namespace StudyMate.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")] // 1. FIX: Changed to the standard /api/[controller] convention
    [ApiController]
    public class NotesController : Controller
    {

        private readonly NotesServices _notesService;

        public NotesController(NotesServices notesService)
        {
            // 3. Store the instance provided by the framework
            _notesService = notesService;
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

        [Authorize]
        [HttpPost("Add", Name = "Addnote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<NoteStatusResponseDTO> Addnote([FromBody] AddNoteDTO note) //To Be Modified
        {
            int authenticatedUserId = GetAuthenticatedUserId();

            //Validation
            if (!ModelState.IsValid)
            {
                // This will often contain the reason for the 'null' (e.g., failed required field)
                return BadRequest(ModelState);
            }

            if (note == null)
            {
                return BadRequest("The request body could not be parsed as a Note.");
            }

            NoteStatusResponseDTO respone = _notesService.AddNote(note , authenticatedUserId);

            if (respone.Success)
            {
                return Ok(respone);
            }

            return BadRequest(respone);
        }
        //http://localhost:7214/api/Notes/Add

        [Authorize]
        [HttpPut("Update/{noteId}", Name = "UpdateNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<NoteStatusResponseDTO> UpdateNote( [FromBody] UpdateNoteDTO Note)// To be Modified soon
        {
            int noteId = Note.NoteID;
            int authenticatedUserId = GetAuthenticatedUserId();

            if (noteId < 0)
            {
                return BadRequest("noteId must be above zero");
            }

            if (!ModelState.IsValid)
            {
                // This will often contain the reason for the 'null' (e.g., failed required field)
                return BadRequest(ModelState);
            }

            NoteStatusResponseDTO response = _notesService.UpdateNote(authenticatedUserId, noteId, Note);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentNotes()
        {

            int userId = GetAuthenticatedUserId();

           var recentNotes = _notesService.GetRecentNotes(userId);

            return Ok(recentNotes);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<List<ClientNoteDTO>> GetAllNotes()
        {
            //Get the UserId from the Token
            int authenticatedUserId = GetAuthenticatedUserId();

            // 4. Use the secure ID to fetch only their data
            return Ok(_notesService.GetAllNotes(authenticatedUserId));
        }


        [Authorize]
        [HttpDelete("Delete/{noteId}" , Name = "DeleteNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       // [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<NoteStatusResponseDTO> DeleteNote(int noteId)
        {
            var authenticatedUserId = GetAuthenticatedUserId();

            NoteStatusResponseDTO response =  _notesService.DeleteNote(noteId , authenticatedUserId);

            if (response.Success) { 
                return Ok(response);
            }
          
           return  NotFound(response);
            
        }

        
    }
}
