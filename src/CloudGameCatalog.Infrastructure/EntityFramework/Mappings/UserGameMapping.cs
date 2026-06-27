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
        builder.HasIndex(x => new { x.IdUser, x.IdGame }).IsUnique();
        builder.Property(x => x.IdUser).IsRequired();
        builder.Property(x => x.IdGame).IsRequired();
    }
}