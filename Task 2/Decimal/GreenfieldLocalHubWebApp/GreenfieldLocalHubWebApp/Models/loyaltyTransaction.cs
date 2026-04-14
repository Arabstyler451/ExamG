namespace GreenfieldLocalHubWebApp.Models
{
    public class loyaltyTransaction
    {
        public int loyaltyTransactionId { get; set; }
        public int loyaltyAccountId { get; set; }
        public int? ordersId { get; set; }
        public int loyaltyPoints { get; set; }
        public string transactionType { get; set; } // Earn or Redeem
        public DateTime transactionDate { get; set; }

        public loyaltyAccount loyaltyAccount { get; set; }
        public orders? orders { get; set; }
    }
}
