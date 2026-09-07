using Microsoft.AspNetCore.Http.Features;

namespace Mcsg.Api.Areas.Story.Extensions;

using Common.Core.Constants;
using Common.SeedWork.Extensions;
using static Common.SeedWork.Dtos.StorageDto;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileUploadLimit(this IServiceCollection services, MinioDto minio)
    {
        var valueLengthLimit = (int)((double)((minio != null && minio.UploadValueLengthLimit > 0) ? minio.UploadValueLengthLimit : Setting.Default.ValueLengthLimit)).FromMegabytes();
        var multipartBodyLengthLimit = (int)((double)((minio != null && minio.UploadMultipartBodyLengthLimit > 0) ? minio.UploadMultipartBodyLengthLimit : Setting.Default.MultipartBodyLengthLimit)).FromMegabytes();
        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = valueLengthLimit;
            x.MultipartBodyLengthLimit = multipartBodyLengthLimit;
        });

        return services;
    }
}
