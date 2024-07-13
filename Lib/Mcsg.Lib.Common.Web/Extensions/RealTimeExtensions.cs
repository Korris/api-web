using Microsoft.AspNetCore.Builder;

namespace Mcsg.Lib.Common.Web.Extensions;

using RealTime.Hubs;

public static class RealTimeExtensions
{
    public static IApplicationBuilder UseCommonHub(this WebApplication applicationBuilder)
    {
        applicationBuilder.MapHub<CommonHub>("/commonHub");
        return applicationBuilder;
    }
}
