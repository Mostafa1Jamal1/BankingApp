using System.Reflection.Metadata;
using api.DataConfigurations;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
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
            modelBuilder.ApplyConfiguration(new AccountConfig());
            modelBuilder.ApplyConfiguration(new SavingsAccountConfig());
            modelBuilder.ApplyConfiguration(new CheckingAccountConfig());
            modelBuilder.ApplyConfiguration(new TransactionConfig());
        }
    }
}
