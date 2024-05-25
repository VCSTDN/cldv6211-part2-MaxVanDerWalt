using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KhumaloCraft.Models
{
    public class CartItems
    {
        [Key]
        public int CartItemsID { get; set; }
        [ForeignKey("Carts")]
        public int CartsID { get; set; }
        [ForeignKey("MyWork")]
        public int MyWorkID { get; set; }

        public virtual MyWorkModel MyWork { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
