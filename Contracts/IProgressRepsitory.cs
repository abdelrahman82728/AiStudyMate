using StudyMate.API.Models.ModelUserProgress;

namespace StudyMate.API.Contracts
{
    public interface IProgressRepsitory
    {
        // Updates time, notes, or logins for today
        Task UpsertActivityAsync(int userId, int seconds, int notes, int logins);

        // Gets data for the charts
        Task<IEnumerable<UserProgress>> GetLastSevenDaysAsync(int userId);

    }
}
