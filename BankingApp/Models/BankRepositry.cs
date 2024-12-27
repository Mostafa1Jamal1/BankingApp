namespace BankingApp.Models
{
    public static class BankRepositry
    {
        public static List<Account> Accounts { get; set; } = new List<Account>()
        {
            new Account
            {
                Id = 1,
                AccountNumber = "6666666666",
                Name = "user1",
                Balance = 0f,
            },
            new Account
            {
                Id = 2,
                AccountNumber = "7777777777",
                Name = "user2",
                Balance = 5454.5f,
            }
        };
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>
        {
            new Transaction
            {
                Id = 0,
                Type = "testType",
                Amount = 0f,
                TimeStamp = DateTime.Now,
            }
        };
    }
}
