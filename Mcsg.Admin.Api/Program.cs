using System.Reflection;

namespace Mcsg.Admin.Api;

using DTOs.Users;
using Lib.AzureBlobStorage;
using Lib.Common;
using Lib.Common.Constants;
using Lib.Common.Interfaces;
using Lib.Common.Models;
using Lib.Common.Web;
using Lib.Common.Web.Extensions;
using Lib.Common.Web.Extensions.DependencyInjection;
using Lib.Data;
using Lib.Data.Wallet;
using Services;
using Services.Interface;
using Validators;

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

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddDataLibrary(builder.Configuration);
        builder.Services.AddWalletDbContext(builder.Configuration);

        builder.Services.AddIdentity();
        builder.Services.AddSignalR();

        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(builder.Configuration);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary(builder.Configuration);
        builder.Services.AddCommonLibrary(builder.Configuration);
        builder.Services.AddAzureBlobStorage(builder.Configuration);

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IValidator<CreateAdminReq>, CreateAdminValidator>();

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
        app.UserSessionAuthorizationMiddleware();
        app.UseAuthorization();

        app.MapControllers();

        app.UseResponseCaching();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Adm";

    #endregion
}
