using StudyMate.API.Models.ModelsAuth;

namespace StudyMate.API.Contracts
{
    public interface IUsersRepository
    {
        // R - Read: Finds a user by NoteID (needed for secure authorization)
        User GetById(int userId);

        // R - Read: Finds a user by email/username (needed for login/authentication)
        User GetByUsername(string username);

        // R - Read: Checks if a user exists by a given NoteID.
        bool ExistsById(int userId);
    }
}
