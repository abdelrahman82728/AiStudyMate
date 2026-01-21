namespace StudyMate.API.Models.ModelsProgress
{
    public class UserProgressDTO
    {
        public string DayName { get; set; } // e.g., "Mon", "Tue"
        public double HoursSpent { get; set; } // We convert seconds to hours for the chart
        public int NotesCreated { get; set; }
        public int LoginCount { get; set; }
    }
}
