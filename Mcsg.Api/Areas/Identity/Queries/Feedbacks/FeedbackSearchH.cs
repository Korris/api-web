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

namespace Mcsg.Api.Areas.Identity.Queries;

using Commands;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Filters;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class FeedbackSearchH : BaseMinioH, IRequestHandler<FeedbackSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public FeedbackSearchH(IMcsgContext context, ISetting setting, IStorageClient sc) : base(context, setting, sc) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(FeedbackSearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        var q = _context.Available<Feedback>(false);

        #region -- Filter --
        string? keyword = null;

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<FeedbackFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword;
            }
        }

        // Keyword
        if (keyword != null)
        {
            q = q.Where(p => string.IsNullOrWhiteSpace(keyword) || (p.Email + "").Contains(keyword));
        }
        #endregion

        // Paging
        res.TotalRecords = q.Count();
        if (request.Paging)
        {
            q = q.Sort(request.Sort).PageBy(request.Offset, request.PageSize);
        }

        // Result
        var data = await q.Select(p => new
        {
            p.Id,
            p.Type,
            p.Email,
            p.Comment,
            p.CreatedOn,
            p.UserId,
            Resources = p.SystemResources.Select(q => new
            {
                q.Id,
                q.HashId,
                Url = _sc.GetCdnUrl(q.Url, q.BucketName, q.MinioInstance, q.Type),
                q.BucketName,
                q.MinioInstance
            })
        }).ToListAsync(cancellationToken);

        return res.SetSuccess(data);
    }

    #endregion
}
