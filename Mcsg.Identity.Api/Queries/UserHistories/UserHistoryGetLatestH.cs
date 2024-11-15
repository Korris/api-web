#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Queries;

using Commands;
using Common.Domain;
using Common.SeedWork.Responses;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class UserHistoryGetLatestH : BaseH, IRequestHandler<UserHistoryGetLatestR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserHistoryGetLatestH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserHistoryGetLatestR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();
        var userId = await _context.UserNameHistoryAvailable.Where(p => p.UserName == request.NewUserName).Select(p => p.UserId).FirstOrDefaultAsync(cancellationToken);

        var latestUserName = await _context.UserNameHistoryAvailable.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => p.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        var data = new
        {
            UserName = latestUserName,
            UserId = userId
        };

        return res.SetSuccess(data);
    }

    #endregion
}
