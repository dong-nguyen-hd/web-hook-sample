namespace WebHookSample.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng TimeEvent
/// </summary>
public sealed class TimeEventConfig : IEntityTypeConfiguration<Models.TimeEvent>
{
    public void Configure(EntityTypeBuilder<Models.TimeEvent> entity)
    {
        entity.ToTable("TBL_TIME_EVENT");

        entity.Property(x => x.Id).UseIdentityColumn();
        entity.HasKey(x => new { x.Id, x.Uuid });

        entity.Property(x => x.TimeStampUtc).HasColumnType("timestamp without time zone");

        entity.HasIndex(x => new { x.TimeStampUtc, x.ProcessType });
    }
}