namespace GreenfieldLocalHubWebApp.Models
{
    public class categories
    {
        public int categoriesId { get; set; } // Primary key
        public string categoryName { get; set; } // Name of the category, e.g., "Fruits and Vegetables", "Dairy Products", "Meat and Poultry", "Bakery Items etc."

        
        // Navigation property to products, it's nullable because a category may be created before any products are added to it
        public ICollection<products>? products { get; set; } // A category can have multiple products, but each product belongs to only one category
    }
}
