using Mcsg.Lib.Common.Web;
using Mcsg.Lib.Common.Web.Extensions;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Extensions;
using Mcsg.Lib.Common.Web.Extensions.DependencyInjection;
using Mcsg.Lib.Data;
using Mcsg.Lib.Data.Wallet;
using Mcsg.Wallet.Api.Models;
using Mcsg.Wallet.Api.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
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