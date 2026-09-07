namespace Mcsg.Api.Areas.Identity.Services;

using Constants;
using Mcsg.Api.Areas.Identity.Interfaces;

public static class SSORegister
{
    public delegate ISSOService SSOServiceResolver(string socialCode);
    public static IServiceCollection AddSSOService(this IServiceCollection services)
    {
        services.AddScoped<FacebookOAuthService>();
        services.AddScoped<GoogleOAuthService>();
        services.AddScoped<AppleOAuthService>();

        services.AddScoped<SSOServiceResolver>(serviceProvider => key =>
        {
            return key switch
            {
                SocialMediaConstants.Facebook.MediaCode => serviceProvider.GetService<FacebookOAuthService>(),
                SocialMediaConstants.Google.MediaCode => serviceProvider.GetService<GoogleOAuthService>(),
                SocialMediaConstants.Apple.MediaCode => serviceProvider.GetService<AppleOAuthService>(),
                _ => null
            };
        });
        return services;
    }
}
