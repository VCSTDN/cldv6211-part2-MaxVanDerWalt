namespace KhumaloCraft.ModelViews
{
    public class CartsModelView
    {
        public List<CartItemsModelView> CartItems { get; set; } = new List<CartItemsModelView>();
        public decimal CartTotal { get; set; }
    }
}
