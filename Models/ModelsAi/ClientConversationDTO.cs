namespace StudyMate.API.Models.ModelsAi
{
    public class ClientConversationDTO
    {
      
            public int ConversationID { get; set; }
            public string ConversationTitle { get; set; }
            public DateTime DateCreated { get; set; }        
            public DateTime LastMessageTimestamp { get; set; } 
            public string DateLastUpdated { get; set; }   
        
    }
}
