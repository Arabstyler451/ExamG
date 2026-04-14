using System.Net;

namespace GreenfieldLocalHubWebApp.Models
{
    public class orders
    {
        public int ordersId { get; set; }
        public int? addressId { get; set; }
        public string UserId { get; set; }
        public decimal? totalAmount { get; set; }
        public bool delivery { get; set; }
        public bool collection { get; set; }
        public string? deliveryType { get; set; }  
        public string orderStatus { get; set; }
        public DateOnly? orderCollectionDate { get; set; }
        public DateOnly orderDate { get; set; }
        public string? DeliveryStreet { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryCity { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryPostalCode { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order
        public string? DeliveryCountry { get; set; } // Will be automatically filled from the address table if delivery is selected, but can be edited by the user if they want to change the delivery address for this order

        public address? address { get; set; }
        public ICollection<orderProducts>? orderProducts { get; set; }
    }
}
