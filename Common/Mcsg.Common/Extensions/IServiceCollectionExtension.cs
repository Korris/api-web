using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Extensions;

using Mail;

/// <summary>
/// IServiceCollection extension for using [this IServiceCollection] only
/// </summary>
public static class IServiceCollectionExtension
{
    #region -- Methods --

    /// <summary>
    /// Add email sender
    /// </summary>
    /// <param name="service">Service</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddEmailSender(this IServiceCollection service)
    {
        service.AddTransient<IEmailSender, SmtpSender>();

        return service;
    }

    #endregion
}
