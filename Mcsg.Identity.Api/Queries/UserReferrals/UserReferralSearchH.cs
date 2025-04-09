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
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Filters;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class UserReferralSearchH : BaseH, IRequestHandler<UserReferralSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserReferralSearchH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserReferralSearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        var q = _context.Available<UserReferral>().Include(p => p.UserReferee).AsNoTracking();

        #region -- Filter --
        var userId = request.UserId;
        string? keyword = null;

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<UserReferralFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword;

                // CreatedOn
                if (ft.FromDate != null && ft.ToDate != null)
                {
                    var tz = request.TimezoneOffset;
                    var fromDate = ft.FromDate.Value.StartOfDay(tz);
                    var toDate = ft.ToDate.Value.EndOfDay(tz);

                    if (ft.FromDate != null)
                    {
                        q = q.Where(p => fromDate <= p.CreatedOn);
                    }
                    if (ft.ToDate != null)
                    {
                        q = q.Where(p => p.CreatedOn <= toDate);
                    }
                }
            }
        }

        // Keyword
        q = q.WhereIf(!string.IsNullOrWhiteSpace(keyword), p =>
            (p.UserReferee.UserName != null && p.UserReferee.UserName.Contains(keyword!)) ||
            (p.UserReferee.ProfileName != null && p.UserReferee.ProfileName.Contains(keyword!))
        );

        // UserReferrerId
        q = q.Where(p => p.UserReferrerId == userId);
        #endregion

        // Paging
        res.TotalRecords = q.Count();
        if (request.Paging)
        {
            q = q.Sort(request.Sort).PageBy(request.Offset, request.PageSize);
        }

        // Result
        var data = await q
            .Select(p => (new UserReferral
            {
                Id = p.Id,
                UserReferee = new User
                {
                    Id = p.UserRefereeId,
                    UserName = p.UserReferee.UserName,
                    ProfileName = p.UserReferee.ProfileName,
                    Avatar = p.UserReferee.Avatar
                },
                CreatedOn = p.CreatedOn
            }).ToSearchDto())
            .ToListAsync(cancellationToken);

        return res.SetSuccess(data);
    }

    #endregion
}
