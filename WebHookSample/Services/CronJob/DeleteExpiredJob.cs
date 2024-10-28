using Microsoft.EntityFrameworkCore;
using WebHookSample.Domain.Context;

namespace WebHookSample.Services.CronJob;

public sealed class DeleteExpiredJob : CronJobService
{
    #region Properties

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Constructor

    public DeleteExpiredJob(IServiceProvider serviceProvider,
        IScheduleConfig<DeleteExpiredJob> config) : base(config.CronExpression, config.TimeZoneInfo)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region Method

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        string jobId = string.Empty;

        try
        {
            await Task.Delay(Random.Shared.Next(1000, 9999), cancellationToken);
            jobId = RelateText.GenId();
            JobContext.LogWithContext().Information($"{nameof(DeleteExpiredJob)} ({jobId}) is working.");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            // Check if there is a previous job that has already executed this task.
            var value = await context.CronJobFlags.FirstOrDefaultAsync(x => x.Name == $"{nameof(DeleteExpiredJob)}", cancellationToken);
            if (value != null)
            {
                // Don't re-execute the task if it has already been performed within the past 2 minutes.
                var utcNow = DateTime.UtcNow;
                if (utcNow.Subtract(value.ExecuteDatetimeUtc).TotalSeconds <= (2 * 60))
                {
                    JobContext.LogWithContext().Information($"{nameof(DeleteExpiredJob)} ({jobId}) is cancel");
                    return;
                }

                value.ExecuteDatetimeUtc = utcNow;
                context.CronJobFlags.Update(value);
                await context.SaveChangesAsync(cancellationToken);

                await ProcesExpiredJobAsync(context, cancellationToken);
            }
            else
            {
                JobContext.LogWithContext().Information($"{nameof(DeleteExpiredJob)} ({jobId}) is cancel");
            }
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                JobContext.LogWithContext().Information($"{nameof(DeleteExpiredJob)} ({jobId}) is cancel");
            else
                JobContext.LogWithContext().Error($"{nameof(DeleteExpiredJob)} ({jobId}) is fail: {ex.Message}", ex);
        }
    }

    #region Private work

    private async Task ProcesExpiredJobAsync(CoreContext context, CancellationToken cancellationToken)
    {
        DateTime pivot = DateTime.UtcNow.Subtract(TimeSpan.FromDays(45));

        // Delte expired webhook from DB
        await context.WebHooks
            .Where(x => x.TriggerDatetimeUtc <= pivot)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion

    #endregion
}