using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebHookSample.Domain.Context.Config;

/// <summary>
/// Chức năng: cấu hình schema cho bảng TimeEvent
/// </summary>
public sealed class TimeEventConfig : IEntityTypeConfiguration<Models.TimeEvent>
{
    public void Configure(EntityTypeBuilder<Models.TimeEvent> entity)
    {
        entity.ToTable("tbl_time_event");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.TimeStampUtc).HasColumnType("timestamp without time zone");

        entity.HasIndex(x => new { x.TimeStampUtc, x.ProcessType });
    }
}