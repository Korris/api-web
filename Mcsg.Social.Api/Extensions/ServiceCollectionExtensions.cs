using Mcsg.Social.Api.Constants;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Common.Extensions;
using Microsoft.AspNetCore.Http.Features;

namespace Mcsg.Social.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileUploadLimit(this IServiceCollection services, IConfiguration configuration)
        {
            var setting = configuration.GetSection("FileSettings").Get<FileSetting>();

            var valueLengthLimit = (int)((double)((setting != null && setting.UploadValueLengthLimit > 0) ? setting.UploadValueLengthLimit : SystemConfig.DefaultValueLengthLimit)).FromMegabytes();
            var multipartBodyLengthLimit = (int)((double)((setting != null && setting.UploadMultipartBodyLengthLimit > 0) ? setting.UploadMultipartBodyLengthLimit : SystemConfig.DefaultMultipartBodyLengthLimit)).FromMegabytes();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = valueLengthLimit;
                x.MultipartBodyLengthLimit = multipartBodyLengthLimit;
            });

            return services;
        }
    }
}
