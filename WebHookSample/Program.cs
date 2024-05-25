using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using WebHookSample.Controllers.Config;
using WebHookSample.Domain.Context;
using WebHookSample.Extensions.AddConfig;

const string _hostingSourceContext = "Microsoft.Hosting.Lifetime";

try
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    var builder = WebApplication.CreateBuilder(args);

    // Declare external-file
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddJsonFile("response-message.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddUserSecrets<Program>(false); // Explicit use secrets.json in env production, staging. By default it only use in development

    SystemGlobal.IsDebug = builder.Environment.IsDevelopment();
    builder.Services.GetSystemData(builder.Configuration);

    // Declare log
    builder.Services.AddLog(builder.Configuration);
    _hostingSourceContext.LogWithContext().Information("Starting up");
    builder.Host.UseSerilog();

    // Force convert dateTime with kind in PostgreSQL
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    #region Add services to the container.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers(opt =>
    {
        opt.ApplyProfile(); // Add custom cache profile

    }).ConfigureApiBehaviorOptions(options =>
    {
        // Adds a custom error response factory when Model-State is invalid
        options.InvalidModelStateResponseFactory = InvalidResponseFactory.ProduceErrorResponse;
    });

    // Add redis / mem cache
    if (CacheConfig.UseMemoryCache)
    {
        builder.Services.AddDistributedMemoryCache();
    }
    else
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = CacheConfig.RedisUri;
            options.InstanceName = CacheConfig.RedisInstanceName;
        });
    }

    builder.Services.AddResponseCaching();
    builder.Services.AddCustomizeSwagger();
    builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<CoreContext>(opts =>
    {
        opts.UseNpgsql(SystemGlobal.PostgresqlConnectionString, o =>
        {
            o.EnableRetryOnFailure();

            if (SystemGlobal.IsDebug == true)
            {
                opts.EnableDetailedErrors();
                opts.EnableSensitiveDataLogging();
            }
        }).UseSnakeCaseNamingConvention();
    });
    builder.Services.AddDependencyInjection(builder.Configuration);
    builder.Services.RegisterHttpClient(builder.Configuration);
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
    });

    #endregion

    #region Configure the HTTP request pipeline.
    var app = builder.Build();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    if (app.Environment.IsProduction())
        app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseRouting();
    app.UseResponseCaching();
    //app.UseMiddleware<RequestResponseLoggerMiddleware>();
    //app.UseMiddleware<ErrorHandlerMiddleware>();
    app.Use((context, next) => // No-caching explicit
    {
        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
        {
            NoCache = true,
            NoStore = true
        };
        return next.Invoke();
    });
    app.MapControllers();
    app.Run();
    #endregion
}
catch (Exception ex)
{
    //if (ex is HostAbortedException) // Ex throw by ef-core when migration
    //    return;

    _hostingSourceContext.LogWithContext().Fatal($"Unhandled exception: {ex.Message}", ex);
}
finally
{
    _hostingSourceContext.LogWithContext().Information("Shut down complete");
    Log.CloseAndFlush();
}
