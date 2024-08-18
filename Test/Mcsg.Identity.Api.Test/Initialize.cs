using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Identity.Api.Test;

using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Extensions;
using Interfaces;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Initialize
/// </summary>
public class Initialize
{
    #region -- Methods --

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="prefix">Variable prefix</param>
    /// <returns>Return the result</returns>
    public static ServiceCollection Init(string prefix)
    {
        // Get assembly name
        var me = typeof(Initialize);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = prefix;

        st.UserNameChangedInRemaining = 0.1;
        st.UserNameWaitingChangedAfter = 0.5;

        // Connection string
        var cs = "Server={DbServer};Database={DbName};Port={DbPort};User Id={DbUser};Password={DbPassword};MaxPoolSize=100;MinPoolSize=10;ConnectionLifetime=300;";

        // Update connection string
        var csDb = cs.SetDbParams(st.Db);

        ServiceCollection services = new();

        #region -- Setup DI --
        // Setting
        services.AddSingleton<ISetting>(st!);

        // DbContext
        services.AddDbContext<McsgContext>(p => p.UseNpgsql(csDb!, p => p.MigrationsAssembly(assembly).EnableRetryOnFailure()), ServiceLifetime.Scoped);
        services.AddScoped<IMcsgContext>(p => p.GetService<McsgContext>()!);

        #endregion

        return services;
    }

    #endregion
}
