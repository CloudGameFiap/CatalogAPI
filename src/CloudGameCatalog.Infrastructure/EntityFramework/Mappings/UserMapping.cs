using CloudGameCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Mappings;

public sealed class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(k => k.Id); 
        builder.Property(p => p.Id)
            .ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired().HasColumnType("VARCHAR(120)");
        builder.Property(p => p.Email).IsRequired().HasColumnType("VARCHAR(120)");
        builder.Property(p => p.BirthDate).HasColumnType("DATETIME2");
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(p => p.UpdateAt).HasColumnType("DATETIME2");
        builder.Property(p => p.Active).HasColumnType("BIT");
        builder.Property(p => p.IsAdmin).HasColumnType("BIT");
    }
}
