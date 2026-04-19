using GreenfieldLocalHubWebApp.Models;

namespace GreenfieldLocalHubWebApp.ViewModels
{
    public class productDetailsViewModel
    {
        public products Product { get; set; }
        public IEnumerable<products> RelatedProducts { get; set; } = new List<products>();
    }
}
