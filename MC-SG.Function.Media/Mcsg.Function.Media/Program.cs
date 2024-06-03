using Mcsg.Function.Media.Constants;
using Mcsg.Function.Media.Services;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.AzureBlobStorage.Settings;
using Mcsg.Lib.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        string connectionString = Environment.GetEnvironmentVariable(FunctionConstant.DbConnectionString);
        services.AddDataLibrary(connectionString);

        services.AddAzureBlobStorage(new AzureBlobStorageSettings
        {
            StorageName = Environment.GetEnvironmentVariable(FunctionConstant.StorageName),
            AccountKey = Environment.GetEnvironmentVariable(FunctionConstant.AccountKey)
        });

        services.AddSingleton<IMediaService, MediaService>();
    })
    .Build();

host.Run();
