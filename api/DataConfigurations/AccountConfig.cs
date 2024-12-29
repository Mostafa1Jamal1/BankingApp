using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Configurations
{
    // Base Account Configuration
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Table Mapping
            builder.ToTable("Accounts");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.AccountNumber).IsRequired().HasMaxLength(34); // IBAN length
            builder.Property(x => x.Balance).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(5);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETDATE()").ValueGeneratedOnUpdate();

            // Unique Constraint on AccountNumber
            builder.HasIndex(x => x.AccountNumber).IsUnique();

            // Discriminator column for TPH mapping
            builder.HasDiscriminator<string>("AccountType")
                   .HasValue<Account>("Base")
                   .HasValue<SavingsAccount>("Savings")
                   .HasValue<CheckingAccount>("Checking");
        }
    }

    // Savings Account Configuration
    public class SavingsAccountConfig : IEntityTypeConfiguration<SavingsAccount>
    {
        public void Configure(EntityTypeBuilder<SavingsAccount> builder)
        {
            builder.Property(x => x.InterestRate).IsRequired().HasColumnType("decimal(6,3)");

            builder.HasData(
            new SavingsAccount
            {
                Id = 1,
                Name = "Mostafa Corp",
                AccountNumber = "6666666666",
                Balance = 500000.00m,
                Currency = "USD",
                Status = "Active",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                InterestRate = 0.15,
            }
            );

        }
    }

    // Checking Account Configuration
    public class CheckingAccountConfig : IEntityTypeConfiguration<CheckingAccount>
    {
        public void Configure(EntityTypeBuilder<CheckingAccount> builder)
        {
            builder.Property(x => x.OverdraftLimit).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasData(
            new CheckingAccount
            {
                Id = 2,
                Name = "Mostafa Personal",
                AccountNumber = "7777777777",
                Balance = 100000.00m,
                Currency = "USD",
                Status = "Active",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                OverdraftLimit = 500,
            }
            );
        }
    }
}
