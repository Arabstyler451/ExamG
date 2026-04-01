namespace GreenfieldLocalHubWebApp.Models
{
    public class shoppingCart
    {
        public int shoppingCartId { get; set; }
        public string UserId { get; set; }
        public DateTime shoppingCartCreatedAt { get; set; } = DateTime.Now;
        public bool shoppingCartStatus { get; set; }

        public ICollection<shoppingCartItems>? shoppingCartItems { get; set; }

    }
}
