using Microsoft.EntityFrameworkCore;
using StudyMate.API.Contracts;
using StudyMate.API.DataAccess.Context;
using StudyMate.API.Models.ModelsAuth;
using StudyMate.API.Models.ModelsNotes;

namespace StudyMate.API.DataAccess.Repositories
{
    public class NotesRepository :INotesRepository
    {
        private readonly ApplicationDbContext _context;

        // The DbContext is injected via Dependency Injection
        public NotesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Note Add(Note note) // Validation so the system doesnt halt
        {
            _context.Notes.Add(note);
            _context.SaveChanges();
            // The 'note' object now contains the database-generated Id
            return note;
        }

        public bool ExistsByHeaderAndUserId(string header, int userId)
        {
            // EF Core translates this LINQ to a fast SQL query
            return _context.Notes.Any(n => n.Header == header && n.UserID == userId);
        }

        public Note GetByIdAndUserId(int noteId, int userId)
        {
            // Find the note, ensuring it belongs to the specified user
            return _context.Notes.FirstOrDefault(n => n.NoteID == noteId && n.UserID == userId);
        }

        private bool _NoteExists(Note note)
        {
            return (_context.Notes.Any(n => (n.NoteID == note.NoteID) && (n.UserID == note.UserID)));
        }

        public bool Update(Note note)
        {

            if (_NoteExists(note) )
            {
            _context.Notes.Update(note);
            _context.SaveChanges();
                return true;
            }

            return false;
        }

        public  List<NoteHandlerDTO> GetRecentNotes(int userId)
        {
            return  _context.Notes
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.LastUpdated) // Or a CreatedAt column if you have one
                .Take(3)
                .Select(n => new NoteHandlerDTO
                {
                    Id = n.NoteID,
                    Title = n.Header, // Mapping 'Header' to 'Title'
                    LastUpdated = n.LastUpdated,                  // Converting the ID to a string name. 
                                      // If you have an Enum, use: ((enSubjectType)n.SubjectTypeID).ToString()
                    Type = n.SubjectTypeID.ToString(),
                })
                .ToList();
        }

        public List<Note> GetNotesByUser(int userID) { 
           return _context.Notes.Where(n => n.UserID == userID).Select(n => n).ToList();
        }
        public bool Delete(int noteId, int userId) {
            Note note = GetByIdAndUserId(noteId, userId);

            if (note != null)
            {
                _context.Notes.Remove(note);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
