using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMate.API.Models.ModelsAuth
{

    
    public class User
    {
        [Key]
        [Column("UserID")]
        public int UserID { set; get;}
        public string UserName { get; set; }
        public string Password { get; set; }
        //public User(string username, string password)
        //{
        //    UserName = username;
        //    Password = password;
        //}
            
        
    }
}
