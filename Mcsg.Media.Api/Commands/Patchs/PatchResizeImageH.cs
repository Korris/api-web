using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Media.Api.Commands;

using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PatchResizeImageH : BaseMinioH, IRequestHandler<PatchResizeImageR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public PatchResizeImageH(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PatchResizeImageR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var comicThumbnailUrls = await _context.ComicPostAvailable.AsNoTracking().Select(p => p.ThumbnailUrl).ToListAsync(cancellationToken);
        var storyThumbnailUrls = await _context.StoryPostAvailable.AsNoTracking().Select(p => p.ThumbnailUrl).ToListAsync(cancellationToken);

        var thumbnailUrls = comicThumbnailUrls.Union(storyThumbnailUrls);
        var bucketName = _sc.GetStrategy(request.MinioInstance).BucketNamePublic;
        var publicUrl = _setting.GetMinio(request.MinioInstance).GetPublicUrl(bucketName, null);
        var count = 0;

        foreach (var i in thumbnailUrls)
        {
            if (string.IsNullOrWhiteSpace(i))
            {
                continue;
            }

            var resizeName = i.Replace(publicUrl, "");
            var fsResize = await _sc.GetStrategy(request.MinioInstance).GetObject(resizeName, bucketName);
            if (fsResize == null)
            {
                continue;
            }

            // Backup the original image
            var originalName = resizeName.AppendNameSuffix();
            var stat = await _sc.GetStrategy(request.MinioInstance).StatObject(originalName, bucketName);
            if (stat == null)
            {
                await _sc.GetStrategy(request.MinioInstance).PutObject(fsResize, originalName, bucketName);
            }

            // Skip processing if the original file name is longer than the resized file name
            var fsOriginal = await _sc.GetStrategy(request.MinioInstance).GetObject(originalName, bucketName);
            if (fsOriginal?.Length >= fsResize.Length)
            {
                continue;
            }

            // Resize the image
            var fs = fsResize.ResizeImage(288, 432, 100);
            if (fs == null)
            {
                continue;
            }

            // No resizing occurred, so backup the original file
            if (fs.Length == fsResize.Length && fsOriginal != null)
            {
                fs = fsOriginal;
            }

            await _sc.GetStrategy(request.MinioInstance).PutObject(fs, resizeName, bucketName);
            count++;
        }

        var data = $"Update {count} file(s)";

        return res.SetSuccess(data);
    }

    #endregion
}
