namespace GreenfieldLocalHubWebApp.Models
{
    public class shoppingCartItems
    {
        public int shoppingCartItemsId { get; set; }
        public int shoppingCartId { get; set; }
        public int productsId { get; set; }
        public float unitPrice { get; set; }
        public int quantity { get; set; }

        public shoppingCart shoppingCart { get; set; }
        public products products { get; set; }
    }
}
