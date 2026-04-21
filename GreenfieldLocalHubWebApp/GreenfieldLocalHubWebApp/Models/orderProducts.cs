using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GreenfieldLocalHubWebApp.Models
{
    public class orderProducts
    {
        public int orderProductsId { get; set; } // Primary Key
        public int ordersId { get; set; } // Foreign key to orders, stores the order that this product is part of
        public int productsId { get; set; } // Foreign key to products, stores the product that is part of this order
        public int quantity { get; set; } // Stores the quantity of this product that is part of this order
        public float unitPrice { get; set; } // Stores the price of this product at the time of the order, in case the price changes in the future

        public orders orders { get; set; } // Navigation properties for orders
        public products products { get; set; } // Navigation properties for products

    }
}
