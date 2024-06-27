using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Mcsg.Media.Api.Commands;

using Common.SeedWork.Responses;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Helpers;
using Lib.Data;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PatchUpdateShareUrlH : IRequestHandler<PatchUpdateShareUrlR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public PatchUpdateShareUrlH(McsgDbContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PatchUpdateShareUrlR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var resources = await _context.Resources.Where(p => p.ShareUrl == null).ToListAsync(cancellationToken);

        foreach (var resource in resources)
        {
            var url = HttpUtility.UrlDecode(resource.Url);
            if (url == null)
            {
                continue;
            }

            var mediaContainer = $"{BlobStorageDefinition.MediaContainer}/";
            url = CryptoHelper.Decrypt(url, _setting.Minio.MediaEncryptKey);

            // Remove duplicates
            if (url.Contains(mediaContainer))
            {
                url = url.Replace(mediaContainer, "");
                resource.Url = UrlHelper.CreateMediaUrl(url, _setting.Minio.MediaEncryptKey);
            }

            resource.ShareUrl = $"{mediaContainer}{url}";
        }

        await _context.SaveChangesAsync();

        var data = $"Update {resources.Count} record(s)";
        res.SetSuccess(data);

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
