using StudyMate.API.Models.ModelsNotes;

namespace StudyMate.API.Contracts
{
    public interface INotesRepository
    {

        // C - Create: Adds a new note to the DB and returns the object with the generated NoteID.
        Note Add(Note note);

        // R - Read: Checks for existence based on header and owner NoteID.
        bool ExistsByHeaderAndUserId(string header, int userId);

        // R - Read: Example to retrieve a single note by its NoteID and the owner's NoteID for security.
        Note GetByIdAndUserId(int noteId, int userId);

        // U - Update: Example to update an existing note.
        bool Update(Note note);

        List<Note> GetNotesByUser(int userID);

        bool Delete(int noteId , int userId);

        List<NoteHandlerDTO> GetRecentNotes(int userId);


    }
}
