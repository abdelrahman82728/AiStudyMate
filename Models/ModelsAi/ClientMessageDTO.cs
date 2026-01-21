namespace StudyMate.API.Models.ModelsAi
{
    public class ClientMessageDTO
    {
        public int MessageID { get; set; }
        public int ConversationID { get; set; }
        public string SenderType { get; set; }
        public string MessageContent { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
