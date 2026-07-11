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

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Ignore(p => p.Name);
        builder.Ignore(p => p.Email);
        builder.Ignore(p => p.BirthDate);
        builder.Ignore(p => p.CreatedAt);
        builder.Ignore(p => p.UpdateAt);
        builder.Ignore(p => p.Active);
        builder.Ignore(p => p.IsAdmin);
    }
}