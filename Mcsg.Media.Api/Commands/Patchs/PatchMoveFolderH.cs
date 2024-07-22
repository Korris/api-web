using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Media.Api.Commands;

using Common.Core.Constants;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PatchMoveFolderH : IRequestHandler<PatchMoveFolderR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public PatchMoveFolderH(McsgContext context, ISetting setting, IStorageClient sc)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PatchMoveFolderR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var qUser = from a in _context.Users
                    join b in _context.UserNameHistories on a.Id equals b.UserId
                    select new
                    {
                        a.Email,
                        a.ProfileId,
                        b.UserName
                    };

        var count = 0;

        var users = await qUser.ToListAsync(cancellationToken);
        var dicUsers = users.GroupBy(p => new { p.Email, p.ProfileId }).ToDictionary(p => p.Key, p => p.Select(q => q.Email).ToList());

        foreach (var i in dicUsers)
        {
            var dstFolder = $"{Setting.MinioFolder.Social}/{i.Key.ProfileId}";
            foreach (var j in i.Value)
            {
                if (string.IsNullOrEmpty(j))
                {
                    continue;
                }

                var srcFolder = $"{Setting.MinioFolder.Social}/{j}";
                count += await _sc.Strategy.MoveFolder(srcFolder, dstFolder, null);
            }
        }

        var data = $"Update {count} folder(s)";
        res.SetSuccess(data);

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    #endregion
}
