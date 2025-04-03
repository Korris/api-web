using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data.Common;
using System.Web;

namespace Mcsg.Social.Api.Queries;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Filters;
using Interfaces;
using Models;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PostSearchHashTagH : BaseMinioH, IRequestHandler<PostSearchHashTagR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="businessText">Business Text</param>
    public PostSearchHashTagH(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessText) : base(context, setting, sc)
    {
        _businessText = businessText;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostSearchHashTagR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        #region -- Filter --
        string? keyword = null;

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<PostFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword;
            }
        }
        #endregion

        var type = request.Type;
        var userId = request.UserId;

        var recordComic = 0;
        var recordSocial = 0;
        var recordStory = 0;
        var recordDocument = 0;

        IEnumerable<PostSeriesTopQueryDbResponse> dataComic = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataSocial = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataStory = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataDocument = [];

        using (var connection = _context.Database.GetDbConnection())
        {
            if (type == PostType.All || type == PostType.Comic)
            {
                var postSeries = await GetPostSeriesTop<ComicPost, ComicTagPost, ComicSubPost>(keyword, request, connection);
                dataComic = postSeries.Item1;
                recordComic = postSeries.Item2;
                await MapReactionPostSeriesTop<ComicPostReaction>(dataComic, userId, request);
            }

            if (type == PostType.All || type == PostType.Document)
            {
                var postSeries = await GetPostSeriesTop<DocumentPost, DocumentTagPost, DocumentSubPost>(keyword, request, connection);
                dataDocument = postSeries.Item1;
                recordDocument = postSeries.Item2;
                await MapReactionPostSeriesTop<DocumentPostReaction>(dataDocument, userId, request);
            }

            if (type == PostType.All || type == PostType.Feed)
            {
                var postSeries = await GetPostSeriesTop<SocialPost, SocialTagPost, SocialSubPost>(keyword, request, connection);
                dataSocial = postSeries.Item1;
                recordSocial = postSeries.Item2;
                foreach (var item in dataSocial)
                {
                    await MappingFeedInListResponse(item);
                }

                await MapReactionPostSeriesTop<SocialPostReaction>(dataSocial, userId, request);
            }

            if (type == PostType.All || type == PostType.Story)
            {
                var postSeries = await GetPostSeriesTop<StoryPost, StoryTagPost, StorySubPost>(keyword, request, connection);
                dataStory = postSeries.Item1;
                recordStory = postSeries.Item2;
                await MapReactionPostSeriesTop<StoryPostReaction>(dataStory, userId, request);
            }

            var combinedItems = dataComic.Concat(dataSocial).Concat(dataStory).Concat(dataDocument).ToList();
            res.TotalRecords = recordStory + recordSocial + recordComic + recordDocument;

            var tagDataMap = new Dictionary<string, IEnumerable<PostSeriesTopQueryDbResponse>>
            {
                { "all", combinedItems },
                { "feed", dataSocial },
                { "comic", dataComic },
                { "story", dataStory },
                { "document", dataDocument }
            };

            if (tagDataMap.ContainsKey(request.Tag))
            {
                return res.SetSuccess(tagDataMap[request.Tag]);
            }
        }

        await _context.Database.CloseConnectionAsync();

        return res;
    }

    /// <summary>
    /// MapReactionPostSeriesTopResponse
    /// </summary>
    /// <param name="item"></param>
    /// <param name="reactions"></param>
    private void MapReactionPostSeriesTopResponse(PostSeriesTopQueryDbResponse item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault().Type
        };
    }

    /// <summary>
    /// MappingFeedInListResponse
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private async Task MappingFeedInListResponse(PostSeriesTopQueryDbResponse item)
    {
        item.Body = await _businessText.Process(item.Body);

        if (!string.IsNullOrEmpty(item.SubPostResourceStr) && item.TotalResource > 0)
        {
            var subPostResources = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostResourceStr);
            var subPostHashIds = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostStr);
            var resourceResponses = subPostResources?.Where(p => p != null).OrderBy(p => p.Order).ToList() ?? [];
            item.Resources = [];

            for (int i = 0; i < resourceResponses.Count; i++)
            {
                var resource = resourceResponses[i];
                resource.Url = await _sc.GetCdnUrlAsync(resource.Url, resource.BucketName, resource.MinioInstance, resource.Type);
                resource.SubPostHashId = subPostHashIds[i].HashId;
                item.Resources.Add(resource);
            }
        }
        else if (item.Link != null)
        {
            item.Link = new PostLinkDto
            {
                HashId = item.HashId ?? "",
                Url = item.Url ?? "",
                Type = item.Type.ToDisplay()
            };
            item.Resources =
            [
                new()
                {
                    HashId = item.HashId,
                    Url = item.Url ?? ""
                }
            ];
        }

        if (item is { MetaTitle: not null, MetaDomain: not null })
        {
            item.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDescription) ? HttpUtility.HtmlDecode(item.MetaDescription) : "",
                Domain = item.MetaDomain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaTitle) ? HttpUtility.HtmlDecode(item.MetaTitle) : "",
                Url = item.MetaUrl ?? ""
            };
        }
    }

    private List<ChapterBasicResponse> MappingTopChapter(string subPostStr)
    {
        var listChapter = (JsonConvert.DeserializeObject<List<ChapterBasicResponse>>(subPostStr))?.Where(x => x != null).
            OrderByDescending(x => x.Order).ToList();

        listChapter.ForEach(x =>
        {
            if (x.ViewCount == null) { x.ViewCount = 0; }
        });

        return listChapter;
    }

    /// <summary>
    /// GetPostSeriesTop
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="TP"></typeparam>
    /// <typeparam name="SP"></typeparam>
    /// <param name="keyword"></param>
    /// <param name="request"></param>
    /// <param name="postType"></param>
    /// <param name="connection"></param>
    /// <returns></returns>
    private async Task<Tuple<IEnumerable<PostSeriesTopQueryDbResponse>, int>> GetPostSeriesTop<P, TP, SP>(string? keyword, PostSearchHashTagR request, DbConnection connection) where P : BasePost where TP : BaseTagPost where SP : BaseSubPost
    {
        var fn = "comic.fw_search_hashtag";
        var @params = "@TagName, @PostType, @StatusList, @PageSize, @OffSetPara, @HideList";
        var schema = typeof(P).Name.ToPrefix().ToLower();
        var type = schema.ToEnum(PostType.Feed);

        var paramValues = new
        {
            TagName = keyword,
            PostType = (int)type,
            StatusList = StatusUtils.PostStatusInt,
            PageSize = (int)request.PageSize,
            OffSetPara = (int)request.Offset,
            HideList = request.Hides
        };

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);
        var qTagPost = _context.Set<TP>().Where(p => !p.IsDelete);
        var qSubPost = _context.Set<SP>().Where(p => !p.IsDelete);
        var count = (from qpost in qPost
                     join qtp in qTagPost on qpost.Id equals qtp.PostId
                     join qtag in _context.Tags on qtp.TagId equals qtag.Id
                     join sp in qSubPost on qpost.Id equals sp.PostId into spGroup
                     from sp in spGroup.DefaultIfEmpty()
                     where qtag.Name == keyword
                           && qpost.Type == type
                           && StatusUtils.PostStatuses.Contains(qpost.Status)
                           && qpost.Permission != PostPermission.Private
                           && !request.Hides.Contains((int)qpost.Hide)
                           && (qpost.Type == PostType.Feed ||
                               (sp != null && StatusUtils.PostStatuses.Contains(sp.Status)
                                && sp.Permission != PostPermission.Private && sp.PublishDate < DateTime.UtcNow))
                     select qpost.Id
              ).Distinct().Count();

        var postSeries = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(fn.ToFn("comic", schema, @params), paramValues);

        return new Tuple<IEnumerable<PostSeriesTopQueryDbResponse>, int>(postSeries, count);
    }

    /// <summary>
    /// MapReactionPostSeriesTop
    /// </summary>
    /// <typeparam name="PR"></typeparam>
    /// <param name="dtos"></param>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task MapReactionPostSeriesTop<PR>(IEnumerable<PostSeriesTopQueryDbResponse> dtos, Guid? userId, PostSearchHashTagR request) where PR : BaseReaction
    {
        if (!dtos.Any())
        {
            return;
        }

        var schema = typeof(PR).Name.ToPrefix().ToLower();
        var type = schema.ToEnum(PostType.Feed);

        var qPostReaction = _context.Set<PR>().Where(p => !p.IsDelete);
        var targetIds = dtos.Select(p => p.Id).ToList();
        var q = await (from r in qPostReaction
                       join u in _context.Users on r.AuthorId equals u.Id
                       where targetIds.Contains(r.TargetId) && !u.IsDelete
                       select new
                       {
                           r.Type,
                           r.TargetId,
                           r.AuthorId
                       }).ToListAsync();

        var reactions = q
            .GroupBy(p => new { p.Type, p.TargetId })
            .Select(grouped => new CommentReactionResponseQuery
            {
                Type = grouped.Key.Type,
                TargetId = grouped.Key.TargetId,
                Count = grouped.Count(),
                ReactByCurrent = grouped.Count(p => p.AuthorId == userId)
            })
            .OrderByDescending(p => p.Count)
            .ToList();

        var isNoSocial = type != PostType.Feed;
        foreach (var i in dtos)
        {
            i.IsCensored = !request.IsAdministrator && request.UserId != i.UserId && i.Status == PostStatus.Inactive;
            i.IsBlur = i.Status == PostStatus.Inactive;

            if (isNoSocial)
            {
                i.Chapters = MappingTopChapter(i.SubPostStr);
            }
        }

        if (reactions.Count == 0)
        {
            return;
        }

        foreach (var i in dtos)
        {
            var postReaction = reactions.Where(p => p.TargetId == i.Id).ToList();
            if (postReaction.Count == 0)
            {
                continue;
            }

            MapReactionPostSeriesTopResponse(i, postReaction);
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business Text
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
