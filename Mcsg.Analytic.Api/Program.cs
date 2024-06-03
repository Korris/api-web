using Mcsg.Analytic.Api.Services;
using Mcsg.Lib.Common;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Web.Extensions.DependencyInjection;
using Mcsg.Lib.Data;
using Mcsg.Lib.Data.Analytic;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcsg.Analytic.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddControllers();
            builder.Services.AddCommonLibrary(builder.Configuration);

            builder.Services.AddAnalyticDbContext(builder.Configuration);

            builder.Services.AddDataLibrary(builder.Configuration);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

            //Add Authentication & Authorization Setup
            builder.Services.AddBearerAuthentication(builder.Configuration);
            builder.Services.AddResponseCaching();

            //Register services
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<ITrackingService, TrackingService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseResponseCaching();

            app.Run();
        }
    }
}
