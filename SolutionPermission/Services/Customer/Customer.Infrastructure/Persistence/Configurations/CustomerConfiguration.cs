using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Infrastructure.Persistence.Configurations;

using Customer.Domain.Entities;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
        .HasMaxLength(50)
        .IsRequired();

        builder.Property(x => x.Name)
        .HasMaxLength(200)
        .IsRequired();

        builder.Property(x => x.Email)
        .HasMaxLength(200)
        .IsRequired();

        builder.Property(x => x.CreatedBy)
        .IsRequired();

        builder.Property(x => x.CreatedAt)
        .IsRequired();

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}