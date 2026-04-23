namespace GreenfieldLocalHubWebApp.Models
{
    public class products
    {
        public int productsId { get; set; } // Primary key
        public int producersId { get; set; } // Foreign key to producers, stores the producer that this product belongs to
        public int categoriesId { get; set; } // Foreign key to categories, stores the category that this product belongs to
        public string productName { get; set; } // Name of the product
        public string productDescription { get; set; } // Description of the product
        public int stockQuantity { get; set; } // Quantity of the product available in stock
        public float productPrice { get; set; } // Price of the product
        public bool productAvailability { get; set; } // Indicates whether the product is currently available for purchase (true) or not (false)
        public string productImage { get; set; } // URL to an image representing the product in the shop page
        public string productUnit { get; set; } // Unit of measurement for the product (e.g., "kg", "liters", "pieces")

        public producers? producers { get; set; } // Navigation property to producers, it's nullable because a product may be created before being assigned to a producer
        public categories categories { get; set; } // Navigation property to categories, it's not nullable because a product must belong to a category
        public ICollection<orderProducts>? orderProducts { get; set; } // Navigation property to orderProducts, it's nullable because a product may be created before being included in any orders
        public ICollection<shoppingCartItems>? shoppingCartItems { get; set; } // Navigation property to shoppingCartItems, it's nullable because a product may be created before being added to any shopping carts

    }
}
