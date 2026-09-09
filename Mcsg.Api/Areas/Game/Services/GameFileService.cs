using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Game.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.Game.Constants;
using Mcsg.Api.Areas.Game.Dtos;
using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Requests;
using Mcsg.Api.Interfaces;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Game uploads. Trimmed copy of Comic FileService.UploadFileAsync: file goes straight to the PUBLIC bucket
/// and a temp GameResource row (IsDelete = true) is written, exactly like ComicResources.
/// The row is attached to a post (PostId set, IsDelete = false) when the post is created with the hashId.
/// Object keys are fully server-derived (user folder from DB + random hash + fixed extension).
/// </summary>
public class GameFileService : IGameFileService
{
    #region -- Methods --

    public GameFileService(IMcsgContext context, ISetting setting, IStorageClient sc, ILogger<GameFileService> logger)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _logger = logger;
    }

    /// <summary>
    /// Thumbnail: image only, size limited by SystemSetting "ThumbnailCoverSize",
    /// always re-encoded to JPEG (strips metadata / animation, normalises the content type)
    /// </summary>
    public async Task<UploadFileDto> UploadThumbnailAsync(FileCreateR request)
    {
        var file = RequireFile(request);
        if (!file.IsImageType() || !file.OpenReadStream().IsImage())
        {
            throw new BadRequestException(nameof(E202), E202);
        }
        var maxMb = await _context.GetSettingDouble("ThumbnailCoverSize");
        EnsureSize(file, maxMb);
        var user = await RequireUserAsync(request.UserId);

        var hashId = ResourceConfig.HashLength.GetRandomString();
        var fileName = $"{hashId}{GameConfig.ThumbnailExtension}";
        var objectName = $"{MinioFolder.Game}/{user.UserFolder}/{GameConfig.ThumbnailFolder}/{fileName}";
        var strategy = _sc.GetStrategy(Instance);
        var bucket = strategy.BucketNamePublic;

        var compressed = file.CompressAndConvertToJpeg(_setting.Minio.ImageDownQuality)
                         ?? throw new BadRequestException(nameof(E202), E202);
        long size;
        using (var stream = compressed.Image.OpenReadStream())
        {
            size = stream.Length;
            await strategy.PutObject(stream, objectName, bucket, GameConfig.ThumbnailContentType);
        }
        await EnsureUploadedAsync(strategy, objectName, bucket);

        var resource = await SaveTempResourceAsync(user.Id, hashId, fileName, objectName, bucket, file, ResourceType.Image, compressed.Width, compressed.Height, size);
        _logger.LogInformation("[GAME-UPLOAD] thumbnail user={UserId} object={Object}", user.Id, objectName);
        return Result(resource);
    }

    /// <summary>
    /// Game file: single .html, max size from SystemSettings "GameFileSize" (MB), stored as text/html so the browser renders it.
    /// The HTML is user-controlled code: it must only ever be embedded through a sandboxed iframe on the media domain.
    /// </summary>
    public async Task<UploadFileDto> UploadGameAsync(FileCreateR request)
    {
        var file = RequireFile(request);
        var ext = Path.GetExtension(file.FileName);
        if (!string.Equals(ext, GameConfig.GameFileExtension, StringComparison.OrdinalIgnoreCase))
        {
            throw new BadRequestException(nameof(E202), GameConfig.OnlyHtmlFileMessage);
        }
        EnsureSize(file, await GetGameFileMaxMbAsync());
        var user = await RequireUserAsync(request.UserId);

        var hashId = ResourceConfig.HashLength.GetRandomString();
        var fileName = $"{hashId}{GameConfig.GameFileExtension}";
        var objectName = $"{MinioFolder.Game}/{user.UserFolder}/{GameConfig.GameFolder}/{fileName}";
        var strategy = _sc.GetStrategy(Instance);
        var bucket = strategy.BucketNamePublic;

        using (var stream = file.OpenReadStream())
        {
            await strategy.PutObject(stream, objectName, bucket, GameConfig.GameFileContentType);
        }
        await EnsureUploadedAsync(strategy, objectName, bucket);

        var resource = await SaveTempResourceAsync(user.Id, hashId, fileName, objectName, bucket, file, ResourceType.Other, 0, 0, file.Length);
        _logger.LogInformation("[GAME-UPLOAD] game file user={UserId} object={Object} size={Size}", user.Id, objectName, file.Length);
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

    /// <summary>
    /// Max game file size from SystemSettings, default when missing, never above the fixed ceiling
    /// </summary>
    private async Task<double> GetGameFileMaxMbAsync()
    {
        var maxMb = await _context.GetSettingDouble(GameConfig.GameFileSizeSettingKey);
        if (maxMb <= 0)
        {
            maxMb = GameConfig.GameFileDefaultMaxMb;
        }
        return Math.Min(maxMb, GameConfig.GameFileCeilingMb);
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
    /// Minio SDK 6.0.x can fail PutObject silently; confirm the object exists before handing the URL out.
    /// </summary>
    private static async Task EnsureUploadedAsync(IStorageStrategy strategy, string objectName, string bucket)
    {
        var stat = await strategy.StatObject(objectName, bucket);
        if (stat == null)
        {
            throw new BadRequestException(nameof(E000), "Upload failed, please try again");
        }
    }

    /// <summary>
    /// Temp resource row (IsDelete = true until attached to a post), same convention as Comic
    /// </summary>
    private async Task<GameResource> SaveTempResourceAsync(Guid userId, string hashId, string fileName, string objectName, string bucket,
        IFormFile file, ResourceType type, int width, int height, double compressedSize)
    {
        var resource = new GameResource
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
        await _context.GameResources.AddAsync(resource);
        await _context.SaveChangesAsync(default);
        return resource;
    }

    private UploadFileDto Result(GameResource resource)
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
    private readonly ILogger<GameFileService> _logger;

    #endregion
}
