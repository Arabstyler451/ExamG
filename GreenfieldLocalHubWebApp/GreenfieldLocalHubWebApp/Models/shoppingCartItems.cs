namespace GreenfieldLocalHubWebApp.Models
{
    public class shoppingCartItems
    {
        public int shoppingCartItemsId { get; set; } // Primary key
        public int shoppingCartId { get; set; } // Foreign key to shoppingCart, stores the shopping cart that this item belongs to
        public int productsId { get; set; } // Foreign key to products, stores the product that this item represents
        public float unitPrice { get; set; } // Price of the product at the time it was added to the cart, this is important to keep track of in case the product price changes later
        public int quantity { get; set; } // Quantity of the product that the user wants to purchase, this is important to keep track of for calculating the total price of the cart and for stock management


        // Navigation properties to represent the relationships with shoppingCart and products
        public shoppingCart shoppingCart { get; set; } // Navigation property to shoppingCart, it's not nullable because a shopping cart item must belong to a shopping cart
        public products products { get; set; } // Navigation property to products, it's not nullable because a shopping cart item must represent a product
    }
}
