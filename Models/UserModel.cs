using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Models
{
    public class UserDetails
    {
        [Key]
        public int UserID { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsAdmin { get; set; }


    }
}
