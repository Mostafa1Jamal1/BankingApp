using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Models.Configurations
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
            builder.Property(x => x.AccountNumber).IsRequired().HasMaxLength(34);
            builder.Property(x => x.Balance).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
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
            builder.Property(x => x.InterestRate).IsRequired();
        }
    }

    // Checking Account Configuration
    public class CheckingAccountConfig : IEntityTypeConfiguration<CheckingAccount>
    {
        public void Configure(EntityTypeBuilder<CheckingAccount> builder)
        {
            builder.Property(x => x.OverdraftLimit).IsRequired();
        }
    }
}
