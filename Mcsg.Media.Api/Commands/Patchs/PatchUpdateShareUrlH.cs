using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Mcsg.Media.Api.Commands;

using Common.Core.Constants;
using Common.Domain;
using Common.SeedWork.Responses;
using Interfaces;
using Lib.Common.Helpers;
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
    public PatchUpdateShareUrlH(IMcsgContext context, ISetting setting)
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

        var resources = await _context.SocialResources.Where(p => p.ShareUrl == null).ToListAsync(cancellationToken);
        var users = await _context.Users.Select(p => new { p.Id, p.Email, p.ProfileId }).ToListAsync(cancellationToken);
        var dicUsers = users.ToDictionary(p => p.Id, q => q.ProfileId);

        foreach (var resource in resources)
        {
            var url = HttpUtility.UrlDecode(resource.Url);
            if (url == null)
            {
                continue;
            }

            var mediaContainer = $"{Setting.MinioFolder.Social}/";
            var urlDecrypt = CryptoHelper.Decrypt(url, _setting.EncryptKey);
            if (!string.IsNullOrWhiteSpace(urlDecrypt))
            {
                url = urlDecrypt;
            }

            var arr = url.Split('/');
            if (arr.Length == 3)
            {
                var userFolder = arr[0];
                if (resource.AuthorId != null)
                {
                    userFolder = dicUsers.GetValueOrDefault(resource.AuthorId.Value);
                }
                url = $"{userFolder}/{arr[1]}/{arr[2]}";
            }
            // Remove duplicates
            if (url.Contains(mediaContainer))
            {
                url = url.Replace(mediaContainer, "");
            }

            resource.Url = url;
            resource.ShareUrl = $"{mediaContainer}{url}";
        }

        await _context.SaveChangesAsync(default);

        var data = $"Update {resources.Count} record(s)";
        res.SetSuccess(data);

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
