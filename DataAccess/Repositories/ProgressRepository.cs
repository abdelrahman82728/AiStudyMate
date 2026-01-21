
using Microsoft.Data.SqlClient;
using StudyMate.API.Contracts;
using StudyMate.API.Models.ModelUserProgress;
using Dapper;

namespace StudyMate.API.DataAccess.Repositories
{
    public class ProgressRepository : IProgressRepsitory
    {

        private readonly string _connectionString;

        public ProgressRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        public async Task UpsertActivityAsync(int userId, int seconds, int notes, int logins)
        {
            // The SQL is defined INSIDE the method
            const string sql = @"
        IF EXISTS (SELECT 1 FROM UserProgress WHERE UserID = @userId AND LogDate = CAST(GETUTCDATE() AS DATE))
        BEGIN
            UPDATE UserProgress 
            SET SecondsSpent = SecondsSpent + @seconds,
                NotesCreated = NotesCreated + @notes,
                LoginCount = LoginCount + @logins
            WHERE UserID = @userId AND LogDate = CAST(GETUTCDATE() AS DATE)
        END
        ELSE
        BEGIN
            INSERT INTO UserProgress (UserID, LogDate, SecondsSpent, NotesCreated, LoginCount)
            VALUES (@userId, CAST(GETUTCDATE() AS DATE), @seconds, @notes, @logins)
        END";

            using var db = new SqlConnection(_connectionString);
            // We pass the 4 parameters into the Dapper executor
            await db.ExecuteAsync(sql, new { userId, seconds, notes, logins });
        }


        public async Task<IEnumerable<UserProgress>> GetLastSevenDaysAsync(int userId)
        {
            using var db = new SqlConnection(_connectionString);

            // We select the top 7 rows for this user, ordered by date descending
            const string sql = @"
        SELECT TOP 7 
            ProgressID, 
            UserID, 
            LogDate, 
            SecondsSpent, 
            NotesCreated, 
            LoginCount 
        FROM UserProgress 
        WHERE UserID = @userId 
        ORDER BY LogDate DESC";

            // Dapper automatically maps the columns to your UserProgress model
            return await db.QueryAsync<UserProgress>(sql, new { userId });
        }

    }
}
