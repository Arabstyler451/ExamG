using System.Net;

namespace GreenfieldLocalHubWebApp.Models
{
    public class orders
    {
        public int ordersId { get; set; } // Primary key
        public int? addressId { get; set; } // Foreign key to address, stores the delivery address for this order, it's nullable because if the user selects collection, then there is no delivery address for this order
        public string UserId { get; set; } // Foreign key to AspNetUsers table for personalisation and authorisation, stores the user that placed this order
        public float totalAmount { get; set; } // Stores the total amount for this order, calculated as the sum of the unit price multiplied by the quantity for each product in this order, plus the delivery fee if delivery is selected
        public bool delivery { get; set; } // Stores whether the user selected delivery or collection for this order
        public bool collection { get; set; } // Stores whether the user selected collection or delivery for this order
        public string? deliveryType { get; set; } // Stores the type of delivery selected by the user for this order, which can be "Standard Delivery", "First Class Delivery", or "Next Day Delivery"
        public float deliveryFee { get; set; } // Stores the delivery fee for this order, which is a fixed amount that is added to the total amount if delivery is selected, and is set to 0 if collection is selected
        public string orderStatus { get; set; } // Stores the status of this order, which can be "Pending", "Processing", "Completed", or "Cancelled", and is initially set to "Pending" when the order is created
        public DateOnly? orderCollectionDate { get; set; } // Stores the date that the user selected for collection, it's nullable because if the user selects delivery, then there is no collection date for this order
        public DateOnly orderDate { get; set; } // Stores the date that this order was placed, which is automatically set to the current date when the order is created
        public string? DeliveryStreet { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryCity { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryPostalCode { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryCountry { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order


        public address? address { get; set; } // Navigation property for address, it's nullable because if the user selects collection, then there is no delivery address for this order
        public ICollection<orderProducts>? orderProducts { get; set; }// Navigation property for orderProducts, it's a collection because an order can have multiple products
    }
}
