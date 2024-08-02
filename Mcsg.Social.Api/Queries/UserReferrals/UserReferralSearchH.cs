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

namespace Mcsg.Social.Api.Queries;

using Commands;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Filters;
using Interfaces;
using Mcsg.Common.Domain.Entities;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class UserReferralSearchH : BaseSettingH, IRequestHandler<UserReferralSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public UserReferralSearchH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserReferralSearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        var q = _context.UserReferralAvailable.AsNoTracking();

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
                    var startOfDay = ft.FromDate.Value.StartOfDay();
                    var endOfDay = ft.ToDate.Value.StartOfDay();

                    var startOfDayUtc = startOfDay.AddMinutes(tz);
                    var endOfDayUtc = endOfDay.AddMinutes(tz);

                    if (ft.FromDate != null)
                    {
                        q = q.Where(p => startOfDayUtc <= p.CreatedOn);
                    }
                    if (ft.ToDate != null)
                    {
                        q = q.Where(p => p.CreatedOn <= endOfDayUtc);
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
                }
            }).ToSearchDto(_setting.Minio.MediaApiUrl))
            .ToListAsync(cancellationToken);

        res.SetSuccess(data);
        return res;
    }

    #endregion
}
