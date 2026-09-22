using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Dtos;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Interfaces;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// TapShow uploads. Copy of GameFileService: file goes to the PUBLIC bucket and a temp TapShowResource row
/// (IsDelete = true) is written until a post / segment / character references it by hashId.
/// Image: re-encoded JPEG, limit "ThumbnailCoverSize". Audio: stored as-is, limit "TapShowAudioSize".
/// </summary>
public class TapShowFileService : ITapShowFileService
{
    #region -- Methods --

    public TapShowFileService(IMcsgContext context, ISetting setting, IStorageClient sc, ILogger<TapShowFileService> logger)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _logger = logger;
    }

    /// <summary>
    /// Image only, always re-encoded to JPEG (strips metadata / animation, normalises the content type)
    /// </summary>
    public async Task<UploadFileDto> UploadImageAsync(FileCreateR request)
    {
        var file = RequireFile(request);
        if (!file.IsImageType() || !file.OpenReadStream().IsImage())
        {
            throw new BadRequestException(nameof(E202), E202);
        }
        EnsureSize(file, await _context.GetSettingDouble(TapShowConfig.ImageSizeSettingKey));
        var user = await RequireUserAsync(request.UserId);

        var hashId = ResourceConfig.HashLength.GetRandomString();
        var fileName = $"{hashId}{TapShowConfig.ImageExtension}";
        var objectName = $"{MinioFolder.TapShow}/{user.UserFolder}/{TapShowConfig.ImageFolder}/{fileName}";

        var compressed = file.CompressAndConvertToJpeg(_setting.Minio.ImageDownQuality)
                         ?? throw new BadRequestException(nameof(E202), E202);
        long size;
        string bucket;
        using (var stream = compressed.Image.OpenReadStream())
        {
            size = stream.Length;
            bucket = await PutAsync(stream, objectName, TapShowConfig.ImageContentType);
        }

        var resource = await SaveTempResourceAsync(user.Id, hashId, fileName, objectName, bucket, file, ResourceType.Image, compressed.Width, compressed.Height, size);
        _logger.LogInformation("[TAPSHOW-UPLOAD] image user={UserId} object={Object}", user.Id, objectName);
        return Result(resource);
    }

    /// <summary>
    /// Voice-over audio of a segment: extension whitelist, size from SystemSettings "TapShowAudioSize" (MB), stored with its audio content type
    /// </summary>
    public async Task<UploadFileDto> UploadAudioAsync(FileCreateR request)
    {
        var file = RequireFile(request);
        var ext = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
        if (!TapShowConfig.AudioExtensionAllow.Contains(ext))
        {
            throw new BadRequestException(nameof(E202), TapShowConfig.OnlyAudioFileMessage);
        }
        var maxMb = await _context.GetSettingDouble(TapShowConfig.AudioSizeSettingKey);
        if (maxMb <= 0)
        {
            maxMb = TapShowConfig.AudioDefaultMaxMb;
        }
        EnsureSize(file, Math.Min(maxMb, TapShowConfig.AudioCeilingMb));
        var user = await RequireUserAsync(request.UserId);

        var hashId = ResourceConfig.HashLength.GetRandomString();
        var fileName = $"{hashId}.{ext}";
        var objectName = $"{MinioFolder.TapShow}/{user.UserFolder}/{TapShowConfig.AudioFolder}/{fileName}";

        string bucket;
        using (var stream = file.OpenReadStream())
        {
            bucket = await PutAsync(stream, objectName, TapShowConfig.AudioContentTypes[ext]);
        }

        var resource = await SaveTempResourceAsync(user.Id, hashId, fileName, objectName, bucket, file, ResourceType.Audio, 0, 0, file.Length);
        _logger.LogInformation("[TAPSHOW-UPLOAD] audio user={UserId} object={Object} size={Size}", user.Id, objectName, file.Length);
        return Result(resource);
    }

    #endregion

    #region -- Helpers --

    private static IFormFile RequireFile(FileCreateR request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            throw new NotFoundException(nameof(E201), E201);
        }
        return request.File;
    }

    private static void EnsureSize(IFormFile file, double maxMb)
    {
        if (file.Length > maxMb.FromMegabytes())
        {
            throw new BadRequestException(nameof(E211), string.Format(E211, maxMb));
        }
    }

    private async Task<User> RequireUserAsync(Guid? userId)
    {
        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == userId);
        return user ?? throw new NotFoundException(nameof(E303), E303);
    }

    /// <summary>
    /// Put on the public bucket and confirm with StatObject (Minio SDK 6.0.x can fail PutObject silently). Returns the bucket.
    /// </summary>
    private async Task<string> PutAsync(Stream stream, string objectName, string contentType)
    {
        var strategy = _sc.GetStrategy(Instance);
        var bucket = strategy.BucketNamePublic;
        await strategy.PutObject(stream, objectName, bucket, contentType);
        var stat = await strategy.StatObject(objectName, bucket);
        if (stat == null)
        {
            throw new BadRequestException(nameof(E000), "Upload failed, please try again");
        }
        return bucket;
    }

    /// <summary>
    /// Temp resource row (IsDelete = true until attached), same convention as Game / Comic
    /// </summary>
    private async Task<TapShowResource> SaveTempResourceAsync(Guid userId, string hashId, string fileName, string objectName, string bucket,
        IFormFile file, ResourceType type, int width, int height, double compressedSize)
    {
        var resource = new TapShowResource
        {
            AuthorId = userId,
            HashId = hashId,
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Name = fileName,
            Url = objectName,
            BucketName = bucket,
            MinioInstance = Instance,
            Type = type,
            Width = width,
            Height = height,
            Size = file.Length,
            CompressedSize = compressedSize,
            Status = ResourceStatus.Done,
            IsDelete = true,
            CreatedBy = userId
        };
        await _context.TapShowResources.AddAsync(resource);
        await _context.SaveChangesAsync(default);
        return resource;
    }

    private UploadFileDto Result(TapShowResource resource)
    {
        return new UploadFileDto
        {
            HashId = resource.HashId!,
            Url = _setting.GetMinio(Instance).GetPublicUrl(resource.BucketName, resource.Url),
            Name = resource.Name!,
            Type = resource.Type,
            Width = resource.Width,
            Height = resource.Height,
            Size = resource.Size
        };
    }

    #endregion

    #region -- Fields --

    private const MinioInstanceType Instance = MinioInstanceType.Default;
    private readonly IMcsgContext _context;
    private readonly ISetting _setting;
    private readonly IStorageClient _sc;
    private readonly ILogger<TapShowFileService> _logger;

    #endregion
}
