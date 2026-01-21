namespace StudyMate.API.Models.ModelsNotes
{
    public class ClientNoteDTO
    {
        public int NoteID { set; get; }

        public int SubjectTypeID { set; get; }
        public string Header { set; get; } = string.Empty;

        public DateTime LastUpdated { set; get; }

        public string Body { set; get; } = string.Empty;
    }
}
