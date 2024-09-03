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
        var take = 10;
        var q = _context.ComicResourceAvailable.Where(p => p.Status == ResourceStatus.WaitingForDownload && p.ExternalUrl != null);
        var ts = TimeSpan.FromMinutes(3);

        var resource = await q.Take(take).ToListAsync();
        foreach (var i in resource)
        {
            var sc = _sc.GetStrategy(MinioInstanceType.Blogtruyen);

            var image = await sc.PutObject(i.ExternalUrl + "", i.Url + "", i.BucketName, false, ts);
            if (image == null)
            {
                i.Status = ResourceStatus.NotFound;
                continue;
            }

            i.Size = image.Stream?.Length ?? 0;
            i.CompressedSize = i.Size;
            i.Width = image.Width;
            i.Height = image.Height;
            i.Status = ResourceStatus.Done;

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
