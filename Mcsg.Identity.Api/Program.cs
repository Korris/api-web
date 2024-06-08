using System.Reflection;

namespace Mcsg.Identity.Api;

using Helpers;
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
using Services.Interface;
using Services.Interfaces;
using Services.SSO;

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

        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
        builder.Services.Configure<OtpSetting>(builder.Configuration.GetSection("OtpSetting"));

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddDataLibrary(builder.Configuration);
        builder.Services.AddWalletDbContext(builder.Configuration);

        builder.Services.AddAzureBlobStorage(builder.Configuration);
        builder.Services.AddIdentity<LocalizeIdentityErrorDescriber>();

        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(builder.Configuration);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary(builder.Configuration);
        builder.Services.AddCommonLibrary(builder.Configuration);
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISessionService, SessionService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSSOService();
        builder.Services.AddScoped<IOtpService, OtpService>();
        builder.Services.AddScoped<IUserWalletService, UserWalletService>();
        builder.Services.AddAzureBlobStorage(builder.Configuration);
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

        app.MapControllers();

        app.UseResponseCaching();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Ide";

    #endregion
}
