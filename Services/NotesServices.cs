using StudyMate.API.Contracts;
using StudyMate.API.DataAccess;
using StudyMate.API.DataAccess.Repositories;
using StudyMate.API.Models.ModelsAuth;
using StudyMate.API.Models.ModelsNotes;

namespace StudyMate.API.Services
{
    public class NotesServices
    {

        // 1. DECLARE the private field to hold the repository dependency
        private readonly INotesRepository _notesRepository; 
        private readonly ProgressServices _progressServices; 

        // 2. DEFINE the constructor for Dependency Injection
        // The DI system automatically calls this, providing a concrete NotesRepository instance.
        public NotesServices(INotesRepository notesRepository , ProgressServices progressServices)
        {
            // 3. ASSIGN the injected instance to the private field
            _notesRepository = notesRepository;
            _progressServices = progressServices;
        }

        public List<ClientNoteDTO> GetAllNotes(int userID)
        {
            

           return _notesRepository.GetNotesByUser(userID).Select(note => new ClientNoteDTO
            {
                NoteID = note.NoteID,
                Header = note.Header,
                SubjectTypeID = note.SubjectTypeID,
                LastUpdated = note.LastUpdated,
                Body = note.Body
            }).ToList();

        }

        public List <NoteHandlerDTO> GetRecentNotes(int userId)
        {
            return _notesRepository.GetRecentNotes(userId);
        }

        //public bool ExistsByHeaderandUserID(string Header , int userId)
        //{
        //    return _notesRepository.ExistsByHeaderAndUserId(Header , userId);
        //}
        public NoteStatusResponseDTO AddNote(AddNoteDTO note, int userId)
        {
            // 1. BUSINESS VALIDATION (Check existence BEFORE adding)

            // Check if a recievednote with this header already exists
            if (_notesRepository.ExistsByHeaderAndUserId(note.Header, userId))  
            {
                return new NoteStatusResponseDTO
                {
                    Success = false,
                    Message = "Note with this Header already exists for this user."
                };
            }

            // 2. PREPARE DATA (If validation passed)
            // Mapping to (Note)

            Note mynote = new Note
            {
                NoteID = 0,
                SubjectTypeID = (int)note.Type,
                Header = note.Header,
                Body = note.Body,
                UserID = userId
            };
            Note createdNote = _notesRepository.Add(mynote);

            _progressServices.IncrementNoteCountAsync(userId);

            ClientNoteDTO dto = new ClientNoteDTO
            {
                NoteID = createdNote.NoteID,
                SubjectTypeID = createdNote.SubjectTypeID,
                Header = createdNote.Header,
                LastUpdated = createdNote.LastUpdated,
                Body = createdNote.Body
            };

            // 3. PERSISTENCE (Only happens if validation passed)

            // 4. Success Response
            return new NoteStatusResponseDTO
            {
                Success = true,
                Message = "Note Added Succesfully",
                dto = dto
            };
        }

        public NoteStatusResponseDTO DeleteNote(int noteId, int userId)
        {

            bool response = _notesRepository.Delete(noteId, userId);

            if (response)
            {
                _progressServices.DecrementNoteCountAsync(userId);

                return new NoteStatusResponseDTO
                {
                    Success = true,
                    Message = "Deleted sucsesfully"
                };
            }

            return new NoteStatusResponseDTO
            {
                Success = false,
                Message = "Not Found"
            };
        }

        public NoteStatusResponseDTO UpdateNote(int userId , int noteId , UpdateNoteDTO recievednote)
        {
            Note note = new Note
            {
                NoteID = noteId,
                SubjectTypeID = (int)recievednote.Type,
                Header = recievednote.Header,
                Body = recievednote.Body,
                UserID = userId
            };
            bool response = _notesRepository.Update(note);

            if (response)
            {
                return new NoteStatusResponseDTO
                {
                    Success = true,
                    Message = "Updated sucsesfully",
                     dto = new ClientNoteDTO
                    {
                        NoteID = note.NoteID,
                        SubjectTypeID = note.SubjectTypeID,
                        Header = note.Header,
                        LastUpdated = note.LastUpdated,
                        Body = note.Body
                    }
                };
            }

            return new NoteStatusResponseDTO
            {
                Success = false,
                Message = "Not Found"
            };
        }
    }
}
