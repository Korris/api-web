using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Story.Queries;
using Mcsg.Api.Interfaces;

using Analytic.Application.Protos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Filters;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class MyStorySearchH : BaseSettingH, IRequestHandler<MyStorySearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public MyStorySearchH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(MyStorySearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }
        var userId = request.UserId.Value;

        try
        {
            var q = _context.Available<StoryPost>().AsNoTracking().Where(p => p.UserId == userId);

            #region -- Filter -- 
            string? keyword = null;
            DateTime? frDate = null;
            DateTime? toDate = null;
            List<PostPermission>? permissions = null;
            Dictionary<string, int> viewCounts = [];
            List<Guid> postIds = [];
            var viewResponse = new StoryPostViewRsp();
            var listIds = new List<string>();

            if (request.Filter != null)
            {
                keyword = request.Filter + "";
                var ft = keyword.ToInstNull<PostFilter.Search>();
                if (ft != null)
                {
                    keyword = ft.Keyword;
                    frDate = ft.FrDate;
                    toDate = ft.ToDate;
                    permissions = ft.Permissions;
                }
            }

            // Keyword
            q = q.Where(p => string.IsNullOrWhiteSpace(keyword) || (p.Title + "").ToLower().Contains(keyword.ToLower()));

            #region -- CreatedOn --
            if (frDate != null)
            {
                frDate = frDate.StartOfDayUtc();
                q = q.Where(p => frDate <= p.CreatedOn);
            }

            if (toDate != null)
            {
                toDate = toDate.EndOfDayUtc();
                q = q.Where(p => p.CreatedOn <= toDate);
            }
            #endregion

            #region -- Permission --
            if (permissions == null)
            {
                permissions = [PostPermission.Public, PostPermission.Private, PostPermission.Premium];
                q = q.Where(p => permissions.Contains(p.Permission));
            }
            else
            {
                q = q.Where(p => permissions.Contains(p.Permission));
            }
            #endregion

            res.TotalRecords = q.Count();

            // Sort
            if (request.Sort.Count == 0)
            {
                q = q.OrderByDescending(p => p.CreatedOn);

                if (request.Paging)
                {
                    q = q.Skip(request.Offset).Take(request.PageSize);
                }

                postIds = await q.Select(p => p.Id).ToListAsync(cancellationToken);
                viewResponse = await GetPostViewFrAna(userId, request.PageNum, request.PageSize, true, keyword, postIds, permissions, frDate, toDate);
                if (viewResponse != null && viewResponse.Items != null)
                {
                    viewCounts = viewResponse.Items.ToDictionary(v => v.PostId + "", v => v.NumberOfViews);
                    listIds = viewResponse.Items.Select(v => v.PostId).ToList();
                    q = q.OrderBy(p => listIds.IndexOf(p.Id.ToString()));
                }
            }
            else
            {
                foreach (var i in request.Sort)
                {
                    var descending = i.Direction.Equals(SortDto.Descending, StringComparison.OrdinalIgnoreCase);

                    if (i.Field.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                    {
                        q = descending
                            ? q.OrderByDescending(p => p.CreatedOn)
                            : q.OrderBy(p => p.CreatedOn);

                        if (request.Paging)
                        {
                            q = q.Skip(request.Offset).Take(request.PageSize);
                        }

                        postIds = await q.Select(p => p.Id).ToListAsync(cancellationToken);
                        viewResponse = await GetPostViewFrAna(userId, request.PageNum, request.PageSize, !descending, keyword, postIds, permissions, frDate, toDate);
                        if (viewResponse != null && viewResponse.Items != null)
                        {
                            viewCounts = viewResponse.Items.ToDictionary(v => v.PostId + "", v => v.NumberOfViews);
                            listIds = viewResponse.Items.Select(v => v.PostId).ToList();
                            q = q.OrderBy(p => listIds.IndexOf(p.Id.ToString()));
                        }
                    }
                    else if (i.Field.Equals("NumberOfViews", StringComparison.OrdinalIgnoreCase))
                    {
                        viewResponse = await GetPostViewFrAna(userId, request.PageNum, request.PageSize, !descending, keyword, postIds, permissions, frDate, toDate);
                        if (viewResponse != null && viewResponse.Items != null)
                        {
                            viewCounts = viewResponse.Items.ToDictionary(v => v.PostId + "", v => v.NumberOfViews);
                            listIds = viewResponse.Items.Select(v => v.PostId).ToList();
                            q = _context.Available<StoryPost>().AsNoTracking().Where(p => p.UserId == userId);
                            q = q.Where(p => listIds.Contains(p.Id.ToString())).OrderBy(p => listIds.IndexOf(p.Id.ToString()));
                        }
                    }
                }
            }
            #endregion

            // Result
            var data = await (from post in q
                              select new StoryPost
                              {
                                  Id = post.Id,
                                  HashId = post.HashId,
                                  Title = post.Title,
                                  Body = post.Body,
                                  CreatedOn = post.CreatedOn,
                                  ThumbnailUrl = post.ThumbnailUrl,
                                  IsDelete = post.IsDelete,
                                  Status = post.Status,
                                  Permission = post.Permission,
                                  StorySubPosts = post.StorySubPosts.Where(sp => !sp.IsDelete).ToList(),
                                  User = new User
                                  {
                                      ProfileName = post.User.ProfileName,
                                      UserName = post.User.UserName
                                  },
                                  StoryPostComments = post.StoryPostComments.Where(pc => !pc.IsDelete).Select(p => new StoryPostComment { Id = p.Id }).ToList(),
                                  StoryPostFavorites = post.StoryPostFavorites.Where(pf => !pf.IsDelete).Select(p => new StoryPostFavorite { Id = p.Id }).ToList(),
                                  NumberOfViews = viewCounts.GetValueOrDefault(post.Id.ToString(), 0),
                                  StoryTagPosts = post.StoryTagPosts.Select(tp => new StoryTagPost { Tag = new Tag { Name = tp.Tag.Name } }).ToList()
                              }.ToSearchDto()).ToListAsync(cancellationToken);

            res.SetSuccess(data);
        }
        catch (Exception ex)
        {
            res.SetError(ex.StackTrace);
        }

        return res;
    }

    private async Task<StoryPostViewRsp> GetPostViewFrAna(Guid userId, int pageNumber, int pageSize, bool isAscending, string? keyWord, List<Guid> postIds, List<PostPermission> permissions, DateTime? frDate, DateTime? toDate)
    {
        var res = new StoryPostViewRsp();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new StoryProto.StoryProtoClient(channel);

            var request = new StoryPostViewReq
            {
                UserId = userId.ToString(),
                PageNum = pageNumber,
                PageSize = pageSize,
                IsAscending = isAscending,
                FrDate = frDate.ToString(),
                ToDate = toDate.ToString(),
                KeyWord = keyWord + ""
            };

            if (postIds != null && postIds.Count > 0)
            {
                request.PostIds.AddRange(postIds.Select(p => p.ToString()));
            }

            if (permissions != null && permissions.Count > 0)
            {
                request.Permissions.AddRange(permissions.Select(p => p.ToString()));
            }

            res = await client.PostViewAsync(request);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }

        return res;
    }

    #endregion
}
