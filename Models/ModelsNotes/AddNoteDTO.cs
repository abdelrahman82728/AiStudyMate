using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StudyMate.API.Models.ModelsNotes
{
    
    public class AddNoteDTO 
    {
        // 1. Subject Type Validation
        [Required(ErrorMessage = "The subject type is required.")]
        [JsonPropertyName("subjectTypeID")] // Tells the binder to map "subjectTypeID" from JSON to this C# property
        // The framework implicitly checks if the incoming string matches a valid enum name.
        public enSubjectType Type { get; set; } // The crucial part: using the enum type

        // 2. Header (Title) Validation
        [Required(ErrorMessage = "The note header is required.")]
        [StringLength(30, ErrorMessage = "The header must be between 1 and 30 characters.", MinimumLength = 1)]
        // We use StringLength/MaxLength to prevent it from being "very long." 100 characters is a good standard maximum for a title.
        public string Header { set; get; } = string.Empty;

        // 3. Body Validation
        // Not required for user freedom, but we must enforce a max length to protect the database.
        [MaxLength(4000, ErrorMessage = "The note body is too long.")]
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Text)]

        public string Body { set; get; } = string.Empty;

        // 4. User NoteID Validation (For your temporary setup)
        [Required(ErrorMessage = "The User NoteID is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "The UserID must not be below zero.")]
        public int UserID { set; get; }

    }


}
