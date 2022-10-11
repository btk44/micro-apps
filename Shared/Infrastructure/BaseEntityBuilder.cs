using Shared.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure;

public static class BaseEntityBuilder {
    public static void Build<T>(EntityTypeBuilder<T> entity) where T : BaseEntity
    {
        // entity.Property(e => e.Id).UseIdentityColumn();
        // entity.Property(e => e.Active).IsRequired().HasDefaultValue(true);
        // entity.Property(e => e.Created).IsRequired().HasDefaultValueSql("getdate()");
        // entity.Property(e => e.CreatedBy).IsRequired().HasDefaultValue(-1);
        // entity.Property(e => e.Modified).IsRequired().HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();
        // entity.Property(e => e.ModifiedBy).IsRequired().HasDefaultValue(-1);
    }
}