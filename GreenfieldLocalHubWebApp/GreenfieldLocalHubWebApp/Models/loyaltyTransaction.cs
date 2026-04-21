namespace GreenfieldLocalHubWebApp.Models
{
    public class loyaltyTransaction
    {
        public int loyaltyTransactionId { get; set; } // Primary Key
        public int loyaltyAccountId { get; set; }  // Foreign Key to loyaltyAccount, linking each transaction to a specific loyalty account
        public int? ordersId { get; set; } // Foreign Key to orders, linking transactions to specific orders when points are earned or redeemed through purchases.
        public int loyaltyPoints { get; set; } // Number of points earned or redeemed in this transaction, positive for earning and negative for redemption
        public string transactionType { get; set; } // Earn or Redeem, used to differentiate between points earned through purchases and points redeemed for offers
        public DateTime transactionDate { get; set; } // Timestamp for when the transaction occurred, used for tracking transaction history and calculating points balance over time

        // Navigation properties for loyaltyAccount and orders
        public loyaltyAccount loyaltyAccount { get; set; } // Navigation property to the related loyalty account, allowing access to the account details and points balance for this transaction
        public orders? orders { get; set; } // One-to-many relationship: one loyalty account can have multiple transactions, and one order can be associated with multiple transactions.
    }
}
