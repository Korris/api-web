using Microsoft.AspNetCore.Http.Features;

namespace Mcsg.Social.Api.Extensions
{
    using Constants;
    using Lib.Common.Extensions;
    using static Common.SeedWork.Dtos.StorageDto;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileUploadLimit(this IServiceCollection services, MinioDto minio)
        {
            var valueLengthLimit = (int)((double)((minio != null && minio.UploadValueLengthLimit > 0) ? minio.UploadValueLengthLimit : SystemConfig.DefaultValueLengthLimit)).FromMegabytes();
            var multipartBodyLengthLimit = (int)((double)((minio != null && minio.UploadMultipartBodyLengthLimit > 0) ? minio.UploadMultipartBodyLengthLimit : SystemConfig.DefaultMultipartBodyLengthLimit)).FromMegabytes();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = valueLengthLimit;
                x.MultipartBodyLengthLimit = multipartBodyLengthLimit;
            });

            return services;
        }
    }
}
