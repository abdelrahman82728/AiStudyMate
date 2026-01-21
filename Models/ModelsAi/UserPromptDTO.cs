using System.ComponentModel.DataAnnotations;

namespace StudyMate.API.Models.ModelsAi
{
    public class UserPromptDTO
    {
        //[Required(ErrorMessage = "The MessageID  is required.")]

        //public int MessageID { get; set; }

        [Required(ErrorMessage = "The ConversationID  is required.")]

        public int ConversationID { get; set; }

        [RegularExpression("^(human|ai)$",
        ErrorMessage = "The SenderType must be 'human' or 'ai'.")]
        public string SenderType { get; set; }

        [MaxLength(8000, ErrorMessage = "Message content cannot exceed 4000 characters.")]
        [Required(ErrorMessage = "Message content is required.")]

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Text)]
        public string MessageContent { get; set; }

    }
}
