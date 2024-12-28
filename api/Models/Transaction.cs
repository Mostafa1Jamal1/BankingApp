using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.Models
{
    public class Transaction
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

        // Navigation properties
        public Account FromAccount { get; set; }
        public Account ToAccount { get; set; }
    }
}
