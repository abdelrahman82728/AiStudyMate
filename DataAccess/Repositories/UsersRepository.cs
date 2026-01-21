using StudyMate.API.Contracts;
using StudyMate.API.DataAccess.Context;
using StudyMate.API.Models.ModelsAuth;

namespace StudyMate.API.DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserID == userId);
        }

        public User GetByUsername(string username)
        {
            // IMPORTANT: Use ToLowerInvariant() if your usernames/emails are case-insensitive
            return _context.Users.FirstOrDefault(u => u.UserName == username);
        }

        public bool ExistsById(int userId)
        {
            return _context.Users.Any(u => u.UserID == userId);
        }
    }
}
