namespace GreenfieldLocalHubWebApp.Models
{
    public class address
    {
        public int addressId { get; set; }
        public string UserId { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }

        public ICollection<orders>? orders { get; set; }
    }
}
