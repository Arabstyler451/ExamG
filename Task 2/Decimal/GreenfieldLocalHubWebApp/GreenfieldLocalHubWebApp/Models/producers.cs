namespace GreenfieldLocalHubWebApp.Models
{
    public class producers
    {
        public int producersId { get; set; }
        public string UserId { get; set; }
        public string producerName { get; set; }
        public string producerEmail { get; set; }
        public string producerPhone { get; set; }
        public string producerDescription { get; set; }
        public string producerLocation { get; set; }
        public string producerImage { get; set; }

        public ICollection<products>? products { get; set; }
    }
}
