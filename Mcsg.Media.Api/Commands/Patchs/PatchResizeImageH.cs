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
        var bucketNamePublic = _sc.Strategy.BucketNamePublic;
        var publicUrl = _setting.Minio.GetPublicUrl(bucketNamePublic, null);
        var count = 0;

        foreach (var i in thumbnailUrls)
        {
            if (string.IsNullOrWhiteSpace(i))
            {
                continue;
            }

            var objectName = i.Replace(publicUrl, "");
            var objectNameOriginal = objectName.GetObjectNameSuffix();

            // Extract the object name by removing the public URL prefix
            var ms = await _sc.Strategy.GetObject(objectName, bucketNamePublic);
            if (ms == null)
            {
                continue;
            }

            // Check objectNameOriginal to avoid unnecessary backups
            var stat = await _sc.Strategy.StatObjectAsync(objectNameOriginal, bucketNamePublic);
            if (stat != null)
            {
                continue;
            }

            // Backup the original image
            await _sc.Strategy.PutObject(ms, objectNameOriginal, bucketNamePublic);

            // Resize the image
            var fs = ms.ResizeImage(144, 180, 100);
            if (fs == null)
            {
                continue;
            }
            await _sc.Strategy.PutObject(fs, objectName, bucketNamePublic);
            count++;
        }

        var data = $"Update {count} file(s)";
        res.SetSuccess(data);

        return res;
    }

    #endregion
}
