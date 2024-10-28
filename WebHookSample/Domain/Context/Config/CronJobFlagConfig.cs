using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebHookSample.Services.CronJob;

namespace WebHookSample.Domain.Context.Config;

/// <summary>
/// Role: config shema for CronJobFlag table
/// </summary>
public sealed class CronJobFlagConfig : IEntityTypeConfiguration<Models.CronJobFlag>
{
    public void Configure(EntityTypeBuilder<Models.CronJobFlag> entity)
    {
        entity.ToTable("tbl_cron_job_flag");
        entity.Property(x => x.ExecuteDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Version).IsRowVersion();

        // Indexing
        entity.HasIndex(x => new { x.Name });

        entity.HasData(new[]
        {
            new Models.CronJobFlag
            {
                ExecuteDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(DeleteExpiredJob)}"
            },
            new Models.CronJobFlag
            {
                ExecuteDatetimeUtc = DateTime.UtcNow,
                Name = $"{nameof(ProcessRequestLaterJob)}"
            }
        });
    }
}