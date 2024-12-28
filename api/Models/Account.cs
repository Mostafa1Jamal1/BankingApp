namespace api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SavingsAccount : Account
    {
        public double InterestRate { get; set; }
    }

    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; set; }
    }
}

