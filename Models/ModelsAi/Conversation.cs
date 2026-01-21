namespace StudyMate.API.Models.ModelsAi
{
    public class Conversation
    {
        public int ConversationID { get; set; }
        public string ConversationTitle { get; set; }
        public int UserID { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateLastUpdated { get; set; } = DateTime.UtcNow;

        public List<Message> Messages { get; set; }   // Navigation property //It linkes ll the messages with the forign Key to the conversation 
                                                     // Used in one to many relationships


    }

}
