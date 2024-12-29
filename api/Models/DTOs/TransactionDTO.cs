namespace api.Models.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Deposit", "Withdrawal", "Transfer"
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign keys (nullable)
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
    }
}
