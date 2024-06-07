using Mcsg.Lib.Common.Web;
using Mcsg.Api.Attributes;
using Mcsg.Api.Extensions;
using Mcsg.Api.Helper;
using Mcsg.Api.Models;
using Mcsg.Api.Services;
using Mcsg.Api.Services.Interfaces;
using Mcsg.Api.Validators;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Interfaces;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Extensions;
using Mcsg.Lib.Common.Web.Extensions.DependencyInjection;
using Mcsg.Lib.Data;
using Mcsg.Lib.Data.Analytic;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Wallet;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<FileSetting>(builder.Configuration.GetSection("FileSettings"));
builder.Services.Configure<RealTimeServiceSetting>(builder.Configuration.GetSection("RealTimeServiceSettings"));
builder.Services.Configure<FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

AppSettingsProvider.Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

builder.Services.AddWalletDbContext(builder.Configuration);
builder.Services.AddAnalyticDbContext(builder.Configuration);
builder.Services.AddDataLibrary(builder.Configuration);

//Add Authentication & Authorization Setup
builder.Services.AddBearerAuthentication(builder.Configuration);
builder.Services.AddResponseCaching();

builder.Services.AddCommonWebLibrary(builder.Configuration);
builder.Services.AddCommonLibrary(builder.Configuration);
builder.Services.AddAzureBlobStorage(builder.Configuration);
builder.Services.AddFileUploadLimit(builder.Configuration);
builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<ILinkPreviewService, LinkPreviewService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMetaDataService, MetaDataService>();
builder.Services.AddScoped<MediaOnlyAttribute>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPostReactService, PostReactService>();
builder.Services.AddScoped<ISubPostReactService, SubPostReactService>();
builder.Services.AddScoped<IPostCommentReactService, PostCommentReactService>();
builder.Services.AddScoped<ISubPostCommentReactService, SubPostCommentReactService>();
builder.Services.AddScoped(typeof(IReactService<>), typeof(ReactService<>));
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IComicService, ComicService>();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<ISmartLookupService, SmartLookupService>();
builder.Services.AddScoped<ISmartCountService, SmartCountService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISoundService, SoundService>();
builder.Services.AddScoped<IViewHistoryService, ViewHistoryService>();
builder.Services.AddScoped<IEarningService, EarningService>();
builder.Services.AddScoped<IUserViewService, UserViewService>();
builder.Services.AddScoped<IAffiliateService, AffiliateService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPostLinkService, PostLinkService>();

//validator
builder.Services.AddScoped<IValidator<TagFavorite>, TagFavoriteValidator>();
builder.Services.AddScoped<IValidator<PostFavorite>, PostFavoriteValidator>();
builder.Services.AddScoped<IValidator<PostReport>, PostReportValidator>();

builder.Services.AddScoped<IFavoriteService, FavoriteService>();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
    options.MaxRequestBodyBufferSize = 10 * 1024 * 1024;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
    options.Limits.MaxRequestBufferSize = 10 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
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
app.EnableNpgsqlLegacyTime();

app.Run();