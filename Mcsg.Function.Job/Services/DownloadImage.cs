using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Enums;
using Interfaces;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// DeleteAccount service
/// </summary>
public class DownloadImage : IDownloadImage
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public DownloadImage(IMcsgContext context, ISetting setting, IStorageClient sc)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <returns>Return the result</returns>
    public async Task Run()
    {
        // Take 1 chapter
        var qSubPost = _context.ComicSubPostAvailable.Where(p => p.Status == PostStatus.WaitingForDownload);
        var subPostId = await qSubPost.OrderBy(p => p.Order).Select(p => p.Id).FirstOrDefaultAsync();
        if (subPostId == Guid.Empty)
        {
            return;
        }

        var ts = TimeSpan.FromMinutes(5);
        var qResource = _context.ComicResourceAvailable.Where(p => p.SubPostId == subPostId);
        qResource = qResource.Where(p => p.Status == ResourceStatus.WaitingForDownload && p.ExternalUrl != null);
        var resource = await qResource.ToListAsync();

        foreach (var i in resource)
        {
            var sc = _sc.GetStrategy(i.MinioInstance ?? MinioInstanceType.Default);
            var image = await sc.PutObject(i.ExternalUrl + "", i.Url + "", i.BucketName, false, ts);
            if (image == null)
            {
                i.Status = ResourceStatus.NotFound;
            }
            else
            {
                i.Status = ResourceStatus.Done;

                if (image.Stream != null)
                {
                    i.Size = image.Stream?.Length ?? 0;
                    i.CompressedSize = i.Size;
                    i.Width = image.Width;
                    i.Height = image.Height;
                }
            }

            i.ModifiedOn = DateTime.UtcNow;
            i.ModifiedBy = CreatedBy.System;
        }

        await _context.SaveChangesAsync(default);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    protected readonly IStorageClient _sc;

    #endregion
}
