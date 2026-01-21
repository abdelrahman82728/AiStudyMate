using System.Collections.Generic;
using StudyMate.API.Contracts;
using StudyMate.API.Models.ModelsProgress;
using StudyMate.API.Models.ModelUserProgress;

namespace StudyMate.API.Services
{
    public class ProgressServices
    {

        private readonly IProgressRepsitory _repo; // Your generic repository or Dapper connection

        public ProgressServices(IProgressRepsitory repo)
        {
            _repo = repo;
        }

        public async Task TrackHeartbeatAsync(int userId)
        {
            // Adds 60 seconds (1 minute) per heartbeat
            await UpsertActivity(userId, seconds: 60, notes: 0, logins: 0);
        }

        public async Task IncrementNoteCountAsync(int userId)
        {
            await UpsertActivity(userId, seconds: 0, notes: 1, logins: 0);
        }

        public async Task DecrementNoteCountAsync(int userId)
        {
            await UpsertActivity(userId, seconds: 0, notes: -1, logins: 0);
        }

        public async Task RecordLoginAsync(int userId)
        {
            await UpsertActivity(userId, seconds: 0, notes: 0, logins: 1);
        }

        private async Task UpsertActivity(int userId, int seconds, int notes, int logins)
        {
            

            await _repo.UpsertActivityAsync( userId, seconds, notes, logins);
        }

        public async Task<IEnumerable<UserProgressDTO>> GetWeeklyStatsAsync(int userId)
        {
            var rawData = await _repo.GetLastSevenDaysAsync(userId);

            // 2. Map it to the DTO using LINQ
            var dto = rawData.Select(row => new UserProgressDTO
            {
                // Convert Date to "Mon", "Tue", etc.
                DayName = row.LogDate.ToString("ddd"),

                // Convert seconds to hours (rounded to 1 decimal place)
                HoursSpent = Math.Round(row.SecondsSpent / 3600.0, 1),

                NotesCreated = row.NotesCreated,
                LoginCount = row.LoginCount
            });

            return dto;
        }
    }
}
