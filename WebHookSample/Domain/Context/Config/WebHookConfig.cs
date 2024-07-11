using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebHookSample.Domain.Context.Config;

/// <summary>
/// Role: config shema for WebHook table
/// </summary>
public sealed class WebHookConfig : IEntityTypeConfiguration<Models.WebHook>
{
    public void Configure(EntityTypeBuilder<Models.WebHook> entity)
    {
        entity.ToTable("tbl_web_hook");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.TriggerDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasIndex(x => new { x.TriggerDatetimeUtc, x.IsDone, x.Level });

        entity.HasMany(x => x.TimeEvents).WithOne(y => y.WebHook).HasForeignKey(z => z.WebHookId);
        entity.OwnsOne(x => x.Headers, builder => { builder.ToJson(); });
    }
}