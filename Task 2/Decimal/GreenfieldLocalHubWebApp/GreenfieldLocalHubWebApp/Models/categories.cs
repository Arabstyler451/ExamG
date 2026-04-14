namespace GreenfieldLocalHubWebApp.Models
{
    public class categories
    {
        public int categoriesId { get; set; }
        public string categoryName { get; set; }

        public ICollection<products>? products { get; set; }
    }
}
