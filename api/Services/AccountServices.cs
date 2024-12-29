using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class AccountServices
    {
        private readonly BankingDbContext _db;

        public AccountServices(BankingDbContext db)
        {
            _db = db;
        }

        public async Task<(bool success, string message, Account? account)> ValidateAccount(string accountNumber)
        {
            var account = await _db.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                return (false, "Account not found", null);

            if (account.Status != "Active")
                return (false, "Account is not active", null);

            return (true, string.Empty, account);
        }

        public (bool success, string message, decimal? availableAmount) ValidateWithdrawal(
            Account account, decimal amount)
        {
            switch (account)
            {
                case CheckingAccount checkingAccount:
                    var availableAmount = checkingAccount.Balance + checkingAccount.OverdraftLimit;
                    if (amount > availableAmount)
                        return (false, $"Amount exceeds available balance and overdraft limit. Maximum withdrawal allowed: {availableAmount}", availableAmount);
                    break;

                case SavingsAccount savingsAccount:
                    if (amount > savingsAccount.Balance)
                        return (false, "Insufficient funds in savings account", savingsAccount.Balance);
                    break;

                default:
                    return (false, "Unsupported account type", null);
            }

            return (true, string.Empty, null);
        }

        public async Task<(bool success, string message, Transaction? transaction)> ProcessTransaction(
            Account fromAccount, Account toAccount, decimal amount, string transactionType)
        {
            var transaction = new Transaction
            {
                Type = transactionType,
                Status = "Pending",
                FromAccountId = fromAccount?.Id,
                ToAccountId = toAccount?.Id,
                Amount = amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            using (var dbTransaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the transaction
                    _db.Transactions.Add(transaction);

                    // Update account balance(s)
                    if (fromAccount != null)
                    {
                        fromAccount.Balance -= amount;
                        fromAccount.UpdatedAt = DateTime.UtcNow;
                    }

                    if (toAccount != null)
                    {
                        toAccount.Balance += amount;
                        toAccount.UpdatedAt = DateTime.UtcNow;
                    }

                    await _db.SaveChangesAsync();

                    // Update transaction status to Done
                    transaction.Status = "Done";
                    await _db.SaveChangesAsync();

                    await dbTransaction.CommitAsync();

                    return (true, "Transaction completed successfully", transaction);
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    return (false, "An error occurred while processing the transaction", null);
                }
            }
        }
    }
}
