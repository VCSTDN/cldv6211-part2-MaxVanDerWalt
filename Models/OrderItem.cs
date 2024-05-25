namespace KhumaloCraft.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public decimal Price { get; set; }

        //Navigation Properties
        public virtual Orders Order { get; set; }
        public virtual MyWorkModel MyWork { get; set; }
    }
}
