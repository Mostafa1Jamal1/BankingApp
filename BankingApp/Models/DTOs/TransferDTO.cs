using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models.DTOs
{
    public class TransferDTO
    {
        // This should be modified accourding to the requirements
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be exactly 10 digits.")]
        public required string From { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be exactly 10 digits.")]
        public required string To { get; set; }

        public float Amount { get; set; }
    }
}
