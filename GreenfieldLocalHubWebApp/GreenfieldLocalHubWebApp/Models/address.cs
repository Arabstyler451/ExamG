namespace GreenfieldLocalHubWebApp.Models
{
    public class address 
    {
        public int addressId { get; set; } // Primary key
        public string UserId { get; set; } // Foreign key to AspNetUsers table for personalisation and authorisation
        public string street { get; set; } // Street address, e.g., "123 Main St"
        public string city { get; set; }    // City name, e.g., "Greenfield"
        public string postalCode { get; set; } // Postal code, e.g., "B72 6LL"
        public string country { get; set; } // Country name, e.g., "England, Wales, Scotland"
        public bool IsDefault { get; set; } // Indicates if this is the default address for the user
        public DateTime createdDate { get; set; } // Timestamp for when the address was created, used to sort the addresses in the user interface, with the most recently added address appearing first in the list


        //Navigation property to orders, it's nullable beacuse and address may be created before any orders are placed
        public ICollection<orders>? orders { get; set; } // A user can have multiple orders associated with a single address, but each order can only be associated with one address
    }
}
