using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebHookSample.Domain.Context;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Services.CronJob;

public sealed class ProcessRequestLaterJob : CronJobService
{
    #region Properties

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Constructor

    public ProcessRequestLaterJob(IServiceProvider serviceProvider,
        IScheduleConfig<ProcessRequestLaterJob> config) : base(config.CronExpression, config.TimeZoneInfo)
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
            JobContext.LogWithContext().Information($"{nameof(ProcessRequestLaterJob)} ({jobId}) is working.");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            // Check if there is a previous job that has already executed this task.
            var value = await context.CronJobFlags.FirstOrDefaultAsync(x => x.Name == $"{nameof(ProcessRequestLaterJob)}", cancellationToken);
            if (value != null)
            {
                // Don't re-execute the task if it has already been performed within the past 2 minutes.
                var utcNow = DateTime.UtcNow;
                if (utcNow.Subtract(value.ExecuteDatetimeUtc).TotalSeconds <= (2 * 60))
                {
                    JobContext.LogWithContext().Information($"{nameof(ProcessRequestLaterJob)} ({jobId}) is cancel");
                    return;
                }

                value.ExecuteDatetimeUtc = utcNow;
                context.CronJobFlags.Update(value);
                await context.SaveChangesAsync(cancellationToken);

                var webHookService = scope.ServiceProvider.GetRequiredService<IWebHookService>();
                await ProcessLaterJobAsync(webHookService, context, cancellationToken);
            }
            else
            {
                JobContext.LogWithContext().Information($"{nameof(ProcessRequestLaterJob)} ({jobId}) is cancel");
            }
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                JobContext.LogWithContext().Information($"{nameof(ProcessRequestLaterJob)} ({jobId}) is cancel");
            else
                JobContext.LogWithContext().Error($"{nameof(ProcessRequestLaterJob)} ({jobId}) is fail: {ex.Message}", ex);
        }
    }

    #region Private work

    private async Task ProcessLaterJobAsync(IWebHookService webHookService, CoreContext context, CancellationToken cancellationToken)
    {
        DateTime pivot = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
        DateTime utcNow = DateTime.UtcNow;

        // Get "later" from DB
        var webHooks = await context.WebHooks
            .Where(x => x.IsDone == false && x.Level == ExecutionLevel.Later && x.TriggerDatetimeUtc >= pivot)
            .ToListAsync(cancellationToken);

        // Add webHook job to "soon"
        if (webHooks.Count <= 0) return;
        Parallel.ForEach(webHooks, webHook => webHook.Level = ExecutionLevel.Soon);

        // Webhook implements optimistic concurrency
        await context.SaveChangesAsync(cancellationToken);

        Parallel.ForEach(webHooks, webHook =>
        {
            var executeNow = webHookService.GetExecutionLevel(webHook.TriggerDatetimeUtc, utcNow);
            BackgroundJob.Schedule(() => webHookService.RequestSoonAsync(webHook, cancellationToken), TimeSpan.FromSeconds(executeNow.seconds));
        });
    }

    #endregion

    #endregion
}