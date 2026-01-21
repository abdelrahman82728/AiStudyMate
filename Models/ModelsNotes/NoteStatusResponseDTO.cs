namespace StudyMate.API.Models.ModelsNotes
{
    public class NoteStatusResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ClientNoteDTO dto { get; set; } = null;
    }
}
