using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebHookSample.Domain.Models.ToJson;

namespace WebHookSample.Domain.Context;

public class CoreContext : DbContext
{
    #region Constructor
    public CoreContext() { }
    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }
    #endregion

    #region Properties
    public DbSet<Models.WebHook> WebHooks { get; set; }
    public DbSet<Models.TimeEvent> TimeEvents { get; set; }
    #endregion

    #region Method
    // Use Fluent API
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Finds and runs all your configuration classes in the same assembly as the DbContext
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    #endregion
}