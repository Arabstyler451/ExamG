namespace GreenfieldLocalHubWebApp.Models
{
    public class loyaltyAccount
    {
        public int loyaltyAccountId { get; set; }
        public string UserId { get; set; }
        public int pointsBalance { get; set; }
        public string loyaltyTier { get; set; } // Bronze, Silver, Gold, Platinum
        public string redeemedOffers { get; set; } = string.Empty;
        public string ActiveOffers { get; set; } = string.Empty;
        public string ConsumedOffers { get; set; } = string.Empty; // tracks vouchers already used in orders

        public ICollection<loyaltyTransaction>? loyaltyTransaction { get; set; }
    }
}
