using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Social.Api.Queries;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Interfaces;
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
    public PostSearchHashTagH(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessText, IRepository<Reaction> reactionRepository) : base(context, setting, sc)
    {
        _businessText = businessText;
        _reactionRepository = reactionRepository;
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

        var qComic = "SELECT * FROM comic.fn_search_hashtag(@TagName, @PostType, @StatusList, @PageSize, @OffSetPara, @HideList)";
        var qSocial = "SELECT * FROM social.fn_search_hashtag(@TagName, @PostType, @StatusList, @PageSize, @OffSetPara, @HideList)";
        var qStory = "SELECT * FROM story.fn_search_hashtag(@TagName, @PostType, @StatusList, @PageSize, @OffSetPara, @HideList)";

        var recordComic = 0;
        var recordSocial = 0;
        var recordStory = 0;

        IEnumerable<PostSeriesTopQueryDbResponse> dataComic = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataSocial = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataStory = [];

        using (var connection = _context.Database.GetDbConnection())
        {
            if (request.Tag == "all" || request.Tag == "comic")
            {
                var param = new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Comic,
                    StatusList = StatusUtils.PostStatusInt,
                    PageSize = (int)request.PageSize,
                    OffSetPara = (int)request.Offset,
                    HideList = request.Hides
                };
                dataComic = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qComic, param);

                recordComic = (
                    from qpost in _context.ComicPostAvailable
                    join qtp in _context.ComicTagPosts on qpost.Id equals qtp.PostId
                    join qtag in _context.Tags on qtp.TagId equals qtag.Id
                    where qtag.Name == keyword
                          && qpost.Type == PostType.Comic
                          && StatusUtils.PostStatusInt.Contains((int)qpost.Status)
                          && qpost.Permission != PostPermission.Private
                          && !request.Hides.Contains((int)qpost.Hide)
                    select qpost.Id
                ).Distinct().Count();
            }

            if (request.Tag == "all" || request.Tag == "feed")
            {
                var param = new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Feed,
                    StatusList = StatusUtils.PostStatusInt,
                    PageSize = (int)request.PageSize,
                    OffSetPara = (int)request.Offset,
                    HideList = request.Hides
                };
                dataSocial = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qSocial, param);

                recordSocial = (
                    from qpost in _context.SocialPostAvailable
                    join qtp in _context.SocialTagPostAvailable on qpost.Id equals qtp.PostId
                    join qtag in _context.TagAvailable on qtp.TagId equals qtag.Id
                    where qtag.Name == keyword
                          && qpost.Type == PostType.Feed
                          && StatusUtils.PostStatusInt.Contains((int)qpost.Status)
                          && !request.Hides.Contains((int)qpost.Hide)
                    select qpost.Id
                ).Distinct().Count();

                foreach (var item in dataSocial)
                {
                    await MappingFeedInListResponse(item);
                }
            }

            if (request.Tag == "all" || request.Tag == "story")
            {
                var param = new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Story,
                    StatusList = StatusUtils.PostStatusInt,
                    PageSize = (int)request.PageSize,
                    OffSetPara = (int)request.Offset,
                    HideList = request.Hides
                };
                dataStory = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qStory, param);

                recordStory = (
                    from qpost in _context.StoryPostAvailable
                    join qtp in _context.StoryTagPosts on qpost.Id equals qtp.PostId
                    join qtag in _context.Tags on qtp.TagId equals qtag.Id
                    where qtag.Name == keyword
                          && qpost.Type == PostType.Story
                          && StatusUtils.PostStatusInt.Contains((int)qpost.Status)
                          && qpost.Permission != PostPermission.Private
                          && !request.Hides.Contains((int)qpost.Hide)
                    select qpost.Id
                ).Distinct().Count();
            }
            if (dataSocial.Any())
            {
                var postReactionResponse = await _reactionRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialPostReactions"""), new
                {
                    TargetIds = dataSocial.Select(p => p.Id).ToList(),
                    request?.UserId
                });

                foreach (var item in dataSocial)
                {
                    item.IsCensored = !request.IsAdministrator && request.UserName != item.UserName && item.Status == PostStatus.Inactive;
                    item.IsBlur = item.Status == PostStatus.Inactive;
                }

                if (postReactionResponse.Count() > 0)
                {
                    foreach (var item in dataSocial)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Count > 0)
                        {
                            MapReactionPostSeiresTopResponse(item, postReaction);
                        }
                    }
                }

            }
            if (dataComic.Any())
            {
                var postReactionResponse = await _reactionRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"comic.""ComicPostReactions"""), new
                {
                    TargetIds = dataComic.Select(p => p.Id).ToList(),
                    request?.UserId
                });

                foreach (var item in dataComic)
                {
                    item.IsCensored = !request.IsAdministrator && request.UserId != item.UserId && item.Status == PostStatus.Inactive;
                    item.IsBlur = item.Status == PostStatus.Inactive;
                    item.Chapters = MappingTopChapter(item.SubPostStr);
                }

                if (postReactionResponse.Count() > 0)
                {
                    foreach (var item in dataComic)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Count > 0)
                        {
                            MapReactionPostSeiresTopResponse(item, postReaction);
                        }
                    }
                }
            }

            if (dataStory.Any())
            {
                var postReactionResponse = await _reactionRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"story.""StoryPostReactions"""), new
                {
                    TargetIds = dataStory.Select(p => p.Id).ToList(),
                    request?.UserId
                });

                foreach (var item in dataStory)
                {
                    item.IsCensored = !request.IsAdministrator && request.UserId != item.UserId && item.Status == PostStatus.Inactive;
                    item.IsBlur = item.Status == PostStatus.Inactive;
                    item.Chapters = MappingTopChapter(item.SubPostStr);
                }

                if (postReactionResponse.Count() > 0)
                {
                    foreach (var item in dataStory)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Count > 0)
                        {
                            MapReactionPostSeiresTopResponse(item, postReaction);
                        }
                    }
                }
            }

            var combinedItems = dataComic.Concat(dataSocial).Concat(dataStory).ToList();

            res.TotalRecords = recordStory + recordSocial + recordComic;

            var tagDataMap = new Dictionary<string, IEnumerable<PostSeriesTopQueryDbResponse>>
            {
                { "all", combinedItems },
                { "feed", dataSocial },
                { "comic", dataComic },
                { "story", dataStory }
            };

            if (tagDataMap.ContainsKey(request.Tag))
            {
                return res.SetSuccess(tagDataMap[request.Tag]);
            }
        }

        await _context.Database.CloseConnectionAsync();

        return res;
    }

    private void MapReactionPostSeiresTopResponse(PostSeriesTopQueryDbResponse item, List<CommentReactionResponseQuery> reactions)
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
            var resourceResponses = subPostResources?.Where(p => p != null).OrderBy(p => p.Order).ToList() ?? [];
            item.Resources = [];

            foreach (var i in resourceResponses)
            {
                i.Url = await _sc.GetCdnUrlAsync(i.Url, i.BucketName, i.MinioInstance, i.Type);
                item.Resources.Add(i);
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

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business Text
    /// </summary>
    private readonly IBusinessText _businessText;

    /// <summary>
    /// ReactionRepostiroy
    /// </summary>
    private readonly IRepository<Reaction> _reactionRepository;

    #endregion
}
