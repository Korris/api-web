using System.Reflection;
using System.Text.Json.Serialization;

namespace Mcsg.Wallet.Api;

using Common.Core.Extensions;
using Lib.AzureBlobStorage;
using Lib.Common;
using Lib.Common.Constants;
using Lib.Common.Models;
using Lib.Common.Web;
using Lib.Common.Web.Extensions;
using Lib.Common.Web.Extensions.DependencyInjection;
using Lib.Data;
using Lib.Data.Wallet;
using Models;
using Services;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Program
/// </summary>
public class Program
{
    #region -- Methods --

    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">Arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Load connection string appsettings.json
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        var cs = config.GetConnectionString(_prefix);

        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
        builder.Services.Configure<OtpSetting>(builder.Configuration.GetSection("OtpSetting"));
        builder.Services.Configure<ZaloPaySetting>(builder.Configuration.GetSection(ZaloPaySetting.ConfigName));

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions
                        .ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddDataLibrary(builder.Configuration);
        builder.Services.AddWalletDbContext(builder.Configuration);


        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(builder.Configuration);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary(builder.Configuration);
        builder.Services.AddCommonLibrary(builder.Configuration);
        builder.Services.AddAzureBlobStorage(builder.Configuration);
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());
        builder.Services.AddSignalR(builder.Configuration);

        //add services
        builder.Services.AddScoped<IUserWalletService, UserWalletService>();
        builder.Services.AddScoped<IOtpService, OtpService>();
        builder.Services.AddScoped<ISystemService, SystemService>();
        builder.Services.AddScoped<IBankService, BankService>();
        builder.Services.AddScoped<IPremiumService, PremiumService>();
        builder.Services.AddScoped<IUserPurchaseService, UserPurchaseService>();
        builder.Services.AddScoped<IZaloPayService, ZaloPayService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseApiResponseAndExceptionWrapper();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UserSessionAuthorizationMiddleware();
        app.UseCommonHub();
        app.MapControllers();

        app.UseResponseCaching();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Wal";

    #endregion
}
