using CloudGameCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Mappings;

public sealed class GameMapping : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(k => k.Id);
        builder.Property(p => p.Name).IsRequired().HasColumnType("VARCHAR(120)");
        builder.Property(p => p.Description).IsRequired().HasColumnType("VARCHAR(120)");
        builder.Property(p => p.ImageUrl).IsRequired().HasColumnType("VARCHAR(250)");
        builder.Property(p => p.Genre).IsRequired().HasColumnType("VARCHAR(60)");
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(p => p.ReleaseDate).HasColumnType("DATETIME2");
        builder.Property(p => p.Active).HasColumnType("BIT");
        builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
    }
}
