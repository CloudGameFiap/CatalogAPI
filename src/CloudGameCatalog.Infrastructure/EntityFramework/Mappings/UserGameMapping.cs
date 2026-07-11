using CloudGameCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudGameCatalog.Infrastructure.EntityFramework.Mappings;

public class UserGameMapping : IEntityTypeConfiguration<UserGame>
{
    public void Configure(EntityTypeBuilder<UserGame> builder)
    {
        builder.ToTable("UserGames");
        builder.HasKey(k => k.Id);

        builder.HasOne(x => x.User)
    .WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Game)
            .WithMany()
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.GameId }).IsUnique();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.GameId).IsRequired();

        builder.Property(x => x.Status).HasColumnType("INT").IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
        builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
    }
}