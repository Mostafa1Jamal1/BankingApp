using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Models.Configurations
{
    public class TransactionConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Map to Transactions table
            builder.ToTable("Transactions");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Type).IsRequired().HasMaxLength(10);
            builder.Property(x => x.DeductedFrom).IsRequired().HasMaxLength(34); // Matches IBAN length
            builder.Property(x => x.AddedTo).IsRequired().HasMaxLength(34); // Matches IBAN length
            // Better precision for monetary values
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.TimeStamp).IsRequired();
        }
    }
}
