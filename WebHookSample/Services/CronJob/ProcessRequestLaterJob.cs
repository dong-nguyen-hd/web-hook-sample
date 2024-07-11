namespace WebHookSample.Services.CronJob;

public class ProcessRequestLaterJob : CronJobService
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
            // var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
            // var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ProcessRequestLaterJob)} fail: {ex.Message}", ex);
        }
    }

    #endregion
}