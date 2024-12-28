using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace api.Models.DTOs
{
    public class AccountNumberC
    {
        // This should be modified accourding to the requirements
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be exactly 10 digits.")]
        public required string AccountNumber { get; set; }
    }
    public class AccountDTO : AccountNumberC
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Balance { get; set; }
    }


}
