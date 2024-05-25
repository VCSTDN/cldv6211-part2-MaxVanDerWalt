using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using KhumaloCraft.Enum;

namespace KhumaloCraft.Models
{
    public enum OrderStatus
    {
        Select,
        Pending,
        Processing,
        Delivered
        
    }

    public class Orders
    {
        [Key]
        public int OrderID { get; set; }
        [Display(Name = "Order Number")]
        public int UserOrderNumber { get; set; }
        [Display(Name = "User")]
        public string? UserID { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Order Created")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Order Modified")]
        public DateTime ModifiedDate { get; set; }
        public string? Address { get; set; }

        //Navigation Properties
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual IdentityUser? User { get; set; }
        [Display(Name = "Status")]
        public OrderStatus OrderStatus { get; set; }
    }
}
