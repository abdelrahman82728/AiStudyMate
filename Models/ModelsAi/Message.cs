namespace StudyMate.API.Models.ModelsAi
{
    public class Message
    {
        public int MessageID { get; set; }         
        public int ConversationID { get; set; }    
        public int UserID { get; set; }            
        public string SenderType { get; set; }     
        public string MessageContent { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Conversation Conversation { get; set; }
    }

}
