using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [RegularExpression("Deposit|Withdraw|Transfer", ErrorMessage = "Transaction type must be 'Deposit', 'Withdraw', or 'Transfer'.")]
        public string Type { get; set; }
        [Required]
        public string DeductedFrom { get; set; } // Source account or "Cash"

        [Required]
        public string AddedTo { get; set; } // Target account or "Cash"
        public float Amount { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
