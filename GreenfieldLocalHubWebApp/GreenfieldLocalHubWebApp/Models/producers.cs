namespace GreenfieldLocalHubWebApp.Models
{
    public class producers
    {
        public int producersId { get; set; } // Primary ke
        public string UserId { get; set; } // Foreign key to AspNetUsers for authorisation 
        public string producerName { get; set; } // Name of the producer
        public string producerEmail { get; set; } // Email address of the producer
        public string producerPhone { get; set; } // Phone number of the producer
        public string producerDescription { get; set; } // Description of the producer's business or products
        public string producerLocation { get; set; } // Location of the producer (e.g., city, region)
        public string producerImage { get; set; } // URL to an image representing the producer in the producer's page

        // Navigation property to the products associated with this producer
        public ICollection<products>? products { get; set; }
    }
}
