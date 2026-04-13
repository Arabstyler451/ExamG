namespace GreenfieldLocalHubWebApp.Models
{
    public class loyaltyAccount
    {
        public int loyaltyAccountId { get; set; }
        public string UserId { get; set; }
        public int pointsBalance { get; set; }
        public string loyaltyTier { get; set; } // Bronze, Silver, Gold, Platinum

        public ICollection<loyaltyTransaction>? loyaltyTransaction { get; set; }
    }
}
