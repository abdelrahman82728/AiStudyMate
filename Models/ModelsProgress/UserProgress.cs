namespace StudyMate.API.Models.ModelUserProgress
{
    public class UserProgress
    {      
            public int ProgressID { get; set; }
            public int UserID { get; set; }
            public DateTime LogDate { get; set; }
            public int SecondsSpent { get; set; }
            public int NotesCreated { get; set; }
            public int LoginCount { get; set; }
        
    }
}
