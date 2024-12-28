using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    public class BankingDbContext : DbContext
    {
        // Constructor
        public BankingDbContext(DbContextOptions<BankingDbContext> options)
            : base(options)
        {
        }

        // DbSets for the entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<CheckingAccount> CheckingAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Configurations for the models
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations
            modelBuilder.ApplyConfiguration(new Configurations.AccountConfig());
            modelBuilder.ApplyConfiguration(new Configurations.SavingsAccountConfig());
            modelBuilder.ApplyConfiguration(new Configurations.CheckingAccountConfig());
            modelBuilder.ApplyConfiguration(new Configurations.TransactionConfig());
        }
    }
}
