using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace Mcsg.Comic.Api.Queries;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Responses;
using Filters;
using Interfaces;
using Models;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class SubPostSearchH : BaseSettingH, IRequestHandler<SubPostSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public SubPostSearchH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(SubPostSearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        var vr = new SubPostSearchV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var postId = request.PostId;

        var q = _context.Available<ComicSubPost>(false).Where(p => p.PostId == postId)
            .Select(subPost => new ChapterResponse
            {
                Id = subPost.Id,
                HashId = subPost.HashId + "",
                Name = subPost.Name + "",
                Title = subPost.Title + "",
                PostId = subPost.PostId,
                Order = subPost.Order,
                Sort = subPost.Order,
                Body = HttpUtility.HtmlDecode(subPost.Body + ""),
                IsPremium = subPost.IsPremium,
                IsExclusive = subPost.IsExclusive,
                Status = subPost.Status,
                CreatedOn = subPost.CreatedOn,
                CreatedBy = subPost.CreatedBy,
                UserId = subPost.UserId,
                PublishDate = subPost.PublishDate,
                Permission = subPost.Permission,
                CreatorNote = subPost.CreatorNote + "",
                IsEnableComment = subPost.IsEnableComment,
                CommentCount = _context.Available<ComicSubPostComment>(false).Count(p => p.PostId == subPost.Id)
            });

        #region -- Filter --
        var keyword = "";

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<SubPostFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword + "";
            }
        }

        // Keyword
        q = q.Where(p => string.IsNullOrWhiteSpace(keyword) || (p.Title + "").ToLower().Contains(keyword.ToLower()));

        // Sort
        if (request.Sort.Count == 0)
        {
            request.Sort.Add(new SortDto("Order", SortDto.Ascending));
        }

        res.TotalRecords = q.Count();

        // Paging
        var viewCountSort = request.Sort.FirstOrDefault(p => p.Field.Equals("ViewCount", StringComparison.OrdinalIgnoreCase));
        if (viewCountSort != null)
        {
            var sendIds = new List<Guid>();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sendIds = await q.Select(p => p.Id).ToListAsync(cancellationToken);
            }

            var descending = viewCountSort.Direction.Equals(SortDto.Descending, StringComparison.OrdinalIgnoreCase);

            var rsp = await SortedSearch(postId, descending, request.PageNum, request.PageSize, sendIds);
            if (rsp.Success)
            {
                var receiveIds = rsp.Ids.Select(p => Guid.Parse(p)).ToList();
                q = q.Where(p => receiveIds.Contains(p.Id)).OrderBy(p => receiveIds.IndexOf(p.Id));
            }
        }
        else
        {
            q = q.Sort(request.Sort).PageBy(request.Offset, request.PageSize);
        }
        #endregion

        var data = await q.ToListAsync(cancellationToken);

        return res.SetSuccess(data);
    }

    /// <summary>
    /// SortedSearch
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="descending"></param>
    /// <param name="pagNum"></param>
    /// <param name="pageSize"></param>
    /// <returns>Return the result</returns>
    private async Task<ComicSubSortedSearchRsp> SortedSearch(Guid postId, bool descending, int pagNum, int pageSize, List<Guid> ids)
    {
        var res = new ComicSubSortedSearchRsp();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new ComicSubProto.ComicSubProtoClient(channel);

            var req = new ComicSubSortedSearchReq
            {
                PostId = postId.ToString(),
                Descending = descending,
                PageNum = pagNum,
                PageSize = pageSize,
                Ids = { ids.Select(p => p.ToString()) }
            };

            res = await client.SortedSearchAsync(req);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    #endregion
}
