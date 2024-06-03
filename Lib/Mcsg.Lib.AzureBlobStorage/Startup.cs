using Mcsg.Lib.AzureBlobStorage.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.AzureBlobStorage
{
    public static class Startup
    {
        /// <summary>
        /// Register for azure function, custom DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="azureBlobStorageSettings"></param>
        public static void AddAzureBlobStorage(this IServiceCollection services, AzureBlobStorageSettings azureBlobStorageSettings)
        {
            services.AddSingleton<IAzureBlobStorageService>(s =>
            {
                return new AzureBlobStorageService(azureBlobStorageSettings);
            });

            services.AddSingleton<IAzureBlobStorageQueueService>(s =>
            {
                return new AzureBlobStorageQueueService(azureBlobStorageSettings);
            });
        }

        /// <summary>
        /// Register for web api / web
        /// </summary>
        /// <param name="services"></param>
        public static void AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorage"));
            services.AddSingleton<IAzureBlobStorageQueueService, AzureBlobStorageQueueService>();
            services.AddSingleton<IAzureBlobStorageService, AzureBlobStorageService>();
        }

        /// <summary>
        /// Register for web api / web
        /// </summary>
        /// <param name="services"></param>
        public static void AddAzureBlobStorage(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IAzureBlobStorageService>(s =>
            {
                return new AzureBlobStorageService(connectionString);
            });

            services.AddSingleton<IAzureBlobStorageQueueService>(s =>
            {
                return new AzureBlobStorageQueueService(connectionString);
            });
        }
    }
}
