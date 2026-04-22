namespace GreenfieldLocalHubWebApp.Models
{
    public class loyaltyAccount
    {
        public int loyaltyAccountId { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to AspNetUsers, for linking personalised loyalty account to user
        public int pointsBalance { get; set; } // Total points available for redemption, updated with each transaction
        public string loyaltyTier { get; set; } // Bronze, Silver, Gold, Platinum
        public string redeemedOffers { get; set; } = string.Empty; // Stores redeemed vouchers/offers, used to prevent reuse and track offer history
        public string ActiveOffers { get; set; } = string.Empty; // Stores currently active vouchers/offers, used to display available offers to the user and manage offer lifecycle
        public string ConsumedOffers { get; set; } = string.Empty; // Stores offers that have been redeemed and used, used for tracking offer usage and preventing reuse
        public string? PendingOffer { get; set; } // Offer the user has chosen to apply at checkout, null = none selected



        // Navigation property for related loyalty transactions
        public ICollection<loyaltyTransaction>? loyaltyTransaction { get; set; } // One-to-many relationship: one loyalty account can have multiple transactions
    }
}
