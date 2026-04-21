namespace GreenfieldLocalHubWebApp.Models
{
    public class shoppingCart
    {
        public int shoppingCartId { get; set; } //Primary key
        public string UserId { get; set; } //Foreign key to AspNetUsers table for user association
        public DateTime shoppingCartCreatedAt { get; set; } = DateTime.Now; //Timestamp for when the shopping cart was created
        public bool shoppingCartStatus { get; set; } //Status to indicate if the cart is active (true) or checked out (false)

        //Navigation property to represent the one-to-many relationship with shoppingCartItems
        public ICollection<shoppingCartItems>? shoppingCartItems { get; set; } //A shopping cart can have multiple items, hence the collection

    }
}
