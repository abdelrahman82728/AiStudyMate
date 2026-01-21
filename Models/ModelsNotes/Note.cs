using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudyMate.API.Models.ModelsNotes
{

    [JsonConverter(typeof(JsonStringEnumConverter))] // <-- ADD THIS ATTRIBUTE
    public enum enSubjectType
    {
        Math = 0, Arabic = 1 , English = 2 , IslamicTeachings = 3 ,History = 4,
        Geography = 5 , Chemistry = 6, Phyics = 7 , ComputerScience = 8 , Other = 9
    }
    
    public class Note
    {
        [Key]
        public int NoteID { set; get; }

        //   public enSubjectType Type { get; set; } // The crucial part: using the enum type
        [Column("SubjectTypeID")]
        public int SubjectTypeID { set; get; }
        public string Header { set; get; } = string.Empty;

        public string Body { set; get; } = string.Empty;
        public DateTime LastUpdated { set; get; } = DateTime.UtcNow;
        public int UserID { set; get; }

       //public Note(int id , int SubjectType, string header, string body , int userId) 
       // {
       //     //Validation 
       //     NoteID = id;
       //     SubjectTypeID = SubjectType;
       //     Header = header;
       //     Body = body;
       //     UserID = userId;
       // }

       
    }

    
}
