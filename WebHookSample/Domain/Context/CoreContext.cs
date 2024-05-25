namespace WebHookSample.Domain.Context;

using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class CoreContext : DbContext
{
    #region Constructor
    public CoreContext() { }
    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }
    #endregion

    #region Properties
    public DbSet<Models.WebHook> WebHooks { get; set; }
    public DbSet<Models.Header> Headers { get; set; }
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
