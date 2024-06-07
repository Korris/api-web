using Mcsg.Lib.Common.Web;
using Mcsg.Admin.Api.DTOs.Users;
using Mcsg.Admin.Api.Services;
using Mcsg.Admin.Api.Services.Interface;
using Mcsg.Admin.Api.Validators;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Interfaces;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Extensions;
using Mcsg.Lib.Common.Web.Extensions.DependencyInjection;
using Mcsg.Lib.Data;
using Mcsg.Lib.Data.Wallet;
using System.Reflection;


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