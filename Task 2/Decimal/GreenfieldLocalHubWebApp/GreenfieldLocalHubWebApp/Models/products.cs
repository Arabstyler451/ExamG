namespace GreenfieldLocalHubWebApp.Models
{
    public class products
    {
        public int productsId { get; set; }
        public int producersId { get; set; } // Foreign key to producers
        public int categoriesId { get; set; } // Foreign key to categories
        public string productName { get; set; }
        public string productDescription { get; set; }
        public int stockQuantity { get; set; }
        public decimal? productPrice { get; set; }
        public bool productAvailability { get; set; }
        public string productImage { get; set; }

        public producers producers { get; set; } // Navigation property to producers
        public categories categories { get; set; } // Navigation property to categories
        public ICollection<orderProducts>? orderProducts { get; set; }
        public ICollection<shoppingCartItems>? shoppingCartItems { get; set; }

    }
}
