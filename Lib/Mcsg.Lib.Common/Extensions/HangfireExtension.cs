using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Extensions;

using Enums;

public static class HangfireExtension
{
    public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireDatabaseType = configuration.GetValue<string>("HangfireSettings:DatabaseType");
        var hangfireDatabase = !string.IsNullOrEmpty(hangfireDatabaseType) && hangfireDatabaseType == "PostgreSQL"
                                                    ? HangfireDatabase.PostgreSQL : HangfireDatabase.MicrosoftSQL;
        services.AddHangfireService(configuration, hangfireDatabase);

        return services;
    }
    public static IServiceCollection AddHangfireService(this IServiceCollection services, IConfiguration configuration, HangfireDatabase dbType)
    {
        if (dbType == HangfireDatabase.PostgreSQL)
        {
            var connectionStr = configuration.GetConnectionString("McsgConnectionString");


            if (!string.IsNullOrEmpty(connectionStr))
            {
                var schemaName = configuration.GetValue<string>("HangfireSettings:SchemaName");
                var queuePollInterval = configuration.GetValue<int>("HangfireSettings:DistributedLockTimeout");
                var invisibilityTimeout = configuration.GetValue<int>("HangfireSettings:InvisibilityTimeout");
                var distributedLockTimeout = configuration.GetValue<int>("HangfireSettings:DistributedLockTimeout");

                schemaName = !string.IsNullOrWhiteSpace(schemaName) ? schemaName : "public";
                invisibilityTimeout = invisibilityTimeout > 0 ? invisibilityTimeout : 3;
                queuePollInterval = queuePollInterval > 0 ? queuePollInterval : 1;
                distributedLockTimeout = distributedLockTimeout > 0 ? distributedLockTimeout : 180;

                services.AddHangfire(configuration => configuration
                   .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UsePostgreSqlStorage(connectionStr, new PostgreSqlStorageOptions
                   {
                       SchemaName = schemaName,
                       InvisibilityTimeout = TimeSpan.FromMinutes(invisibilityTimeout),
                       QueuePollInterval = TimeSpan.FromMinutes(queuePollInterval),
                       DistributedLockTimeout = TimeSpan.FromMinutes(distributedLockTimeout),
                   }));


                // Add the processing server as IHostedService
                var schedulePollingInterval = configuration.GetValue<int>("HangfireSettings:SchedulePollingInterval");
                var heartbeatInterval = configuration.GetValue<int>("HangfireSettings:HeartbeatInterval");
                var serverCheckInterval = configuration.GetValue<int>("HangfireSettings:ServerCheckInterval");
                var workerCount = configuration.GetValue<int>("HangfireSettings:WorkerCount");

                schedulePollingInterval = schedulePollingInterval > 0 ? schedulePollingInterval : 10;
                heartbeatInterval = heartbeatInterval > 0 ? heartbeatInterval : 10;
                serverCheckInterval = serverCheckInterval > 0 ? serverCheckInterval : 30;
                workerCount = workerCount > 0 ? workerCount : 10;

                services.AddHangfireServer(options =>
                {
                    options.SchedulePollingInterval = TimeSpan.FromMinutes(schedulePollingInterval);
                    options.HeartbeatInterval = TimeSpan.FromMinutes(heartbeatInterval);
                    options.ServerCheckInterval = TimeSpan.FromMinutes(serverCheckInterval);
                    options.WorkerCount = workerCount;
                });
            }
        }
        else if (dbType == HangfireDatabase.MicrosoftSQL)
        {
            var connectionStr = configuration.GetConnectionString("WalletDbConnectionString");

            if (!string.IsNullOrEmpty(connectionStr))
            {
                var commandBatchMaxTimeout = configuration.GetValue<int>("HangfireSettings:CommandBatchMaxTimeout");
                var slidingInvisibilityTimeout = configuration.GetValue<int>("HangfireSettings:SlidingInvisibilityTimeout");
                var queuePollInterval = configuration.GetValue<int>("HangfireSettings:QueuePollInterval");
                var useRecommendedIsolationLevel = configuration.GetValue<bool>("HangfireSettings:UseRecommendedIsolationLevel");
                var disableGlobalLocks = configuration.GetValue<bool>("HangfireSettings:DisableGlobalLocks");

                commandBatchMaxTimeout = commandBatchMaxTimeout > 0 ? commandBatchMaxTimeout : 5;
                slidingInvisibilityTimeout = slidingInvisibilityTimeout > 0 ? slidingInvisibilityTimeout : 5;
                queuePollInterval = queuePollInterval > 0 ? queuePollInterval : 1;

                services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionStr, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(commandBatchMaxTimeout),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(slidingInvisibilityTimeout),
                    QueuePollInterval = TimeSpan.FromMinutes(queuePollInterval),
                    UseRecommendedIsolationLevel = useRecommendedIsolationLevel,
                    DisableGlobalLocks = disableGlobalLocks,

                }));

                // Add the processing server as IHostedService
                services.AddHangfireServer();
            }
        }

        return services;
    }
    public static IApplicationBuilder UseHangfireService(this WebApplication app, IConfiguration configuration)
    {
        var hangfireDatabaseType = configuration.GetValue<string>("HangfireSettings:DatabaseType");
        var hangfireDatabase = !string.IsNullOrEmpty(hangfireDatabaseType) && hangfireDatabaseType == "PostgreSQL"
                                                    ? HangfireDatabase.PostgreSQL : HangfireDatabase.MicrosoftSQL;
        app.UseHangfireService(configuration, hangfireDatabase);
        return app;
    }
    public static IApplicationBuilder UseHangfireService(this WebApplication applicationBuilder, IConfiguration configuration, HangfireDatabase dbType)
    {
        if (dbType == HangfireDatabase.PostgreSQL)
        {
            applicationBuilder.UseHangfireServer();
        }
        else if (dbType == HangfireDatabase.MicrosoftSQL)
        {
            //HangfireDashboardAuthentication
            var options = new DashboardOptions
            {
                Authorization = new IDashboardAuthorizationFilter[]
                {
            new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    // Case sensitive login checking
                    LoginCaseSensitive = true,
                    // Users
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = configuration.GetValue<string>("HangfireSettings:AdminUsername"),
                            // Password as plain text, SHA1 will be used
                            PasswordClear = configuration.GetValue<string>("HangfireSettings:AdminPassword")
                        }
                    }
                })
                    }
            };
            applicationBuilder.UseHangfireDashboard("/hangfire", options);
        }

        return applicationBuilder;
    }
}
