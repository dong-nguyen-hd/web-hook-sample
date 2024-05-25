namespace WebHookSample.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng WebHook
/// </summary>
public sealed class WebHookConfig : IEntityTypeConfiguration<Models.WebHook>
{
    public void Configure(EntityTypeBuilder<Models.WebHook> entity)
    {
        entity.ToTable("TBL_WEB_HOOK");

        entity.Property(x => x.Id).UseIdentityColumn();
        entity.HasKey(x => new { x.Id, x.Uuid });

        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.TriggerDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasIndex(x => new { x.CreatedDatetimeUtc, x.IsProcess });

        entity.HasMany(x => x.Headers).WithOne(y => y.WebHook).HasForeignKey(z => new { z.WebHookId, z.WebHookUuid });
        entity.HasMany(x => x.TimeEvents).WithOne(y => y.WebHook).HasForeignKey(z => new { z.WebHookId, z.WebHookUuid });
    }
}
