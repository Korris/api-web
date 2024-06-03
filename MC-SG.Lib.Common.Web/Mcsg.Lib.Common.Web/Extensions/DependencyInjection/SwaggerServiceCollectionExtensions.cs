using Mcsg.Lib.Common.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web.Extensions.DependencyInjection
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string scheme)
        {
            services.AddSwaggerGen(options =>
            {
                switch (scheme)
                {
                    case AuthenticationSchemes.JwtScheme:
                        options.AddJwtSecurity();
                        break;
                    case AuthenticationSchemes.ApiKeyScheme:
                        options.AddApiKeySecurity();
                        break;
                    default:
                        break;
                }
            });
            return services;
        }
    }
}
