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

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            Log.Information($"{nameof(ProcessRequestLaterJob)} is working.");

            using var scope = _serviceProvider.CreateScope();
            var webHookService = scope.ServiceProvider.GetRequiredService<IWebHookService>();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            await ProcessLaterJobAsync(webHookService, context, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ProcessRequestLaterJob)} fail: {ex.Message}", ex);
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
        Parallel.ForEach(webHooks, webHook =>
        {
            webHook.Level = ExecutionLevel.Soon;
            var executeNow = webHookService.GetExecutionLevel(webHook.TriggerDatetimeUtc, utcNow);
            BackgroundJob.Schedule(() => webHookService.RequestSoonAsync(webHook, cancellationToken), TimeSpan.FromSeconds(executeNow.seconds));
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #endregion
}