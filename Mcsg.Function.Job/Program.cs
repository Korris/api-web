using Mcsg.Function.Job.Constants;
using Mcsg.Function.Job.Services;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.AzureBlobStorage.Settings;
using Mcsg.Lib.Common.Mail;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data;
using Mcsg.Lib.Data.Wallet;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        string connectionString = Environment.GetEnvironmentVariable(FunctionConstant.DbConnectionString);
        string walletConnectionString = Environment.GetEnvironmentVariable(FunctionConstant.WalletDbConnectionString);
        services.AddDataLibrary(connectionString);
        services.AddWalletDbContext(walletConnectionString);

        services.AddAzureBlobStorage(new AzureBlobStorageSettings
        {
            StorageName = Environment.GetEnvironmentVariable(FunctionConstant.StorageName),
            AccountKey = Environment.GetEnvironmentVariable(FunctionConstant.AccountKey)
        });
        services.Configure<SmtpSettings>(x =>
        {
            x.SmtpHost = Environment.GetEnvironmentVariable(FunctionConstant.EmailHost);
            x.SmtpUser = Environment.GetEnvironmentVariable(FunctionConstant.EmailUser);
            x.SmtpPass = Environment.GetEnvironmentVariable(FunctionConstant.EmailPass);
            x.SmtpFrom = Environment.GetEnvironmentVariable(FunctionConstant.EmailFrom);
            x.SmtpDisplayFrom = Environment.GetEnvironmentVariable(FunctionConstant.EmailDisplayFrom);
            x.SmtpPort = int.Parse(Environment.GetEnvironmentVariable(FunctionConstant.EmailPort));
        });

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped(typeof(ICountService<,>), typeof(CountService<,>));
        services.AddSingleton<IEmailSender, SmtpSender>();
        services.AddScoped<ISyncDataService, SyncDataService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IExclusiveUnlockService, ExclusiveUnlockService>();
        services.AddScoped<IPaymentService, PaymentService>();
    })
    .Build();

host.Run();
