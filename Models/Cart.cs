using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }
        [ForeignKey("User")]
        public string? UserID { get; set; }
        public virtual ICollection<CartItems>? CartItems { get; set; } = new List<CartItems>();
        public virtual IdentityUser? User { get; set; }
    }
}
