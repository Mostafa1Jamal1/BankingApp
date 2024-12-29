using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataConfigurations
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
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18,2)"); // 9 999 999 999 999 999.99 is A LOT
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETDATE()").ValueGeneratedOnUpdate();


            // Relationships
            builder.HasOne(t => t.FromAccount)
                   .WithMany(a => a.OutgoingTransactions)
                   .HasForeignKey(t => t.FromAccountId)
                   .IsRequired(false)  // Makes the relationship optional
                   .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            builder.HasOne(t => t.ToAccount)
                   .WithMany(a => a.IncomingTransactions)
                   .HasForeignKey(t => t.ToAccountId)
                   .IsRequired(false)  // Makes the relationship optional
                   .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete
        }
    }
}
