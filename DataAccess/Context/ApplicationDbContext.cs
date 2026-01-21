using Microsoft.EntityFrameworkCore;
using StudyMate.API.Models;
namespace StudyMate.API.DataAccess.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets representing your tables
        public DbSet<Models.ModelsNotes.Note> Notes { get; set; }
        public DbSet<Models.ModelsAuth.User> Users { get; set; }
        public DbSet<Models.ModelsSubjects.Subject> Subjects { get; set; } 
        public DbSet<Models.ModelsAi.Conversation> Conversations { get; set; } 
        public DbSet<Models.ModelsAi.Message> Messages { get; set; } 
    }
}
