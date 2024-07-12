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

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            Log.Information($"{nameof(DeleteExpiredJob)} is working.");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            await ProcesExpiredJobAsync(context, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(DeleteExpiredJob)} fail: {ex.Message}", ex);
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