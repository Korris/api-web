using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Constants;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Models.Earning;
using Requests;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class PostService : IPostService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="mapper"></param>
    /// <param name="currentUserService"></param>
    /// <param name="smartLookupService"></param>
    public PostService(IMcsgContext context, ISetting setting, IStorageClient sc, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, ISmartLookupService smartLookupService)
    {
        _context = context;
        _setting = setting;
        _sc = sc;

        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _postCommentRepository = unitOfWork.GetRepository<SocialPostComment>();
        _smartLookupRepository = unitOfWork.GetRepository<SmartLookup>();
        _postReportRepository = unitOfWork.GetRepository<SocialPostReport>();
        _mapper = mapper;
        _currentUserService = currentUserService;
        _smartLookupService = smartLookupService;
    }

    public async Task<bool> Delete(Guid postId)
    {
        var ss = _currentUserService.Session;
        var currentUserId = ss.UserId;
        var profileName = ss.ProfileName;

        var feedDb = await _postRepository.GetByIdAsync(postId);
        if (feedDb == null)
        {
            throw new BadRequestException(E204, M204);
        }
        else if (feedDb.UserId != currentUserId)
        {
            throw new BadRequestException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = DateTime.UtcNow, UserId = currentUserId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(postId, profileName);
            return true;
        }
    }

    #region PostStoryOrComic
    public async Task UpdateKeyWordForComicAndStoryToSmartLookup()
    {
        var queryNameListPost = $@"SELECT ""Title"" FROM social.""SocialPosts"" where ""Type"" != {(int)PostType.Feed}  AND ""IsDelete"" = false ";
        var nameListPost = await _postRepository.Connection.QueryAsync<string>(queryNameListPost);
        var smartLookupInserts = new List<SmartLookup>();
        foreach (var name in nameListPost)
        {
            smartLookupInserts.Add(new SmartLookup
            {
                CountCriteria = 0,
                Keyword = name,
                KeywordType = LookupKeywordType.None
            });
        }
        await _smartLookupRepository.InsertAsync(smartLookupInserts);
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_TAG);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    PostStatus = (int)PostStatus.Public,
                    TagName = tagName
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
            results.Items = MappingTopSeries(items);
        }
        else
        {
            results = new PagedResponse<PostSeriesTopResponse>(0);
        }
        return results;
    }

    public async Task<List<MyPostSeriesResponse>> GetMyAllSeries()
    {
        try
        {
            var currentUserId = _currentUserService?.Session?.UserId;
            var query = string.Format(GetMyAllQuery);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        UserId = currentUserId
                    });
            var queryResults = await multi.ReadAsync<MyPostSeriesQueryResult>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();

            if (queryResults != null && queryResults.Count() > 0)
            {
                var items = _mapper.Map<List<MyPostSeriesResponse>>(queryResults);

                return items;
            }
            else
            {
                return new List<MyPostSeriesResponse>();
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<ListIdForHomePage> GetLatestPostsByType()
    {
        try
        {
            //TODO - Will get the percent from config later
            // Get from DB
            var value = 200;
            var feedPercent = .6f;
            var storyPercent = .1f;
            var comicPercent = .3f;

            // Conver to amount
            var feed = (int)Math.Round(value * feedPercent, 0);
            var story = (int)Math.Round(value * storyPercent, 0);
            var comic = (int)Math.Round(value * comicPercent, 0);
            var param = new
            {
                feed,
                story,
                comic,
                feedPercent,
                storyPercent,
                comicPercent
            };

            var query = GetLatestPostsDataByTypeQuery;
            query = query.Replace("[GetTotalCount]", GetCountPostDataByTypeQuery);

            var multi = await _postCommentRepository.Connection.QueryMultipleAsync(query, param);

            var listposts = await multi.ReadAsync<LatestPostsResponse>().ConfigureAwait(false);
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            return new ListIdForHomePage() { TotalItems = totalItems, LatestPostsResponses = listposts.ToList() };
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<ListIdForHomePage> GetLatestPostsByTag(string nameTag)
    {
        try
        {
            //TODO - Will get the percent from config later
            // Get from DB
            var value = 200;
            var feedPercent = .6f;
            var storyPercent = .1f;
            var comicPercent = .3f;

            // Conver to amount
            var feed = (int)Math.Round(value * feedPercent, 0);
            var story = (int)Math.Round(value * storyPercent, 0);
            var comic = (int)Math.Round(value * comicPercent, 0);

            var param = new
            {
                feed,
                story,
                comic,
                feedPercent,
                storyPercent,
                comicPercent,
                ExactKeyword = nameTag
            };

            var query = GetLatestPostsByTagQuery;
            query = query.Replace("[GetTotalCount]", GetCountPostByTagQuery);
            var multi = await _postCommentRepository.Connection.QueryMultipleAsync(query, param);
            var listposts = await multi.ReadAsync<LatestPostsResponse>().ConfigureAwait(false);
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            return new ListIdForHomePage() { TotalItems = totalItems, LatestPostsResponses = listposts.ToList() };
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<List<NewsFeedDto>> GetNewsFeed(UserNamePagingR input)
    {

        var user = await _context.UserAvailable.AsNoTracking()
                                        .Where(p => p.UserName == input.UserName)
                                        .FirstOrDefaultAsync();
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var userFollowingIds = await _context.UserFollows.AsNoTracking()
                                                .Where(p => p.UserFollowerId == user.Id)
                                                .Select(p => p.UserFollowerId)
                                                .ToListAsync();
        var fromDate = DateTime.Today.AddDays(-2);
        var query = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",
                    pc.""CreatedOn"",
                    p.""Type"" , 
                    p.""HashId"" as HashPostId,
                    FALSE as IsSubPost , 
                    NULL as Order
                    from social.""SocialPostComments"" pc 
                    left join social.""SocialPosts"" p on  pc.""PostId"" = p.""Id""
                    left join ""identity"".""Users"" u on pc.""CreatedBy"" = u.""Id""
                    WHERE pc.""CreatedBy"" = ANY(@UserIds)
                    AND pc.""CreatedBy"" != @CurrentUserId
                    AND pc.""CreatedOn"" < now()
                    AND pc.""CreatedOn"" > @FromDate
                    AND p.""IsDelete"" = false
                    AND pc.""IsDelete"" = false
                    Order by ""CreatedOn"" desc
                    LIMIT 2";

        var data = await _postCommentRepository.Connection.QueryAsync<NewsFeedDto>(query, new
        {
            UserIds = userFollowingIds,
            CurrentUserId = user.Id,
            FromDate = fromDate
        });
        var amountDataNeedToTake = data != null ? input.PageSize - data.Count() : input.PageSize;
        var queryDataNeedToTake = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",pc.""CreatedOn"",p.""Type"" , 
                        p.""HashId"" as HashPostId,
                        FALSE as IsSubPost, NULL as Order,
                        COALESCE(COUNT(pcr.""Id""), 0) AS reaction_count,
                         RANDOM() AS sort_key
                        FROM social.""SocialPostComments"" pc 
                        LEFT JOIN social.""SocialPosts"" p on  pc.""PostId"" = p.""Id""
                        LEFT JOIN social.""SocialPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
                        LEFT JOIN ""identity"".""Users"" u on pc.""CreatedBy"" = u.""Id""
                        WHERE pc.""Id"" <> ALL (ARRAY[@CommentIds]) 
                        AND pc.""CreatedBy"" != @CurrentUserId
                        AND p.""IsDelete"" = false
                        AND pc.""IsDelete"" = false
                        AND pc.""CreatedOn"" > @FromDate
                        GROUP BY p.""HashId"", u.""Avatar"",u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",p.""Type""
                      
                        ORDER BY sort_key
                        LIMIT @Limit";

        var commentIds = data.Select(p => p.Id).ToArray();
        var dataNeedToTake = await _postCommentRepository.Connection.QueryAsync<NewsFeedDto>(queryDataNeedToTake, new
        {
            CommentIds = amountDataNeedToTake < input.PageSize ? commentIds : [],
            Limit = amountDataNeedToTake,
            CurrentUserId = user.Id,
            FromDate = fromDate
        });

        return data.Concat(dataNeedToTake).ToList();
    }

    public async Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input)
    {
        var query = "SELECT * FROM social.fn_get_visible_post_maybe_you_like(@Limit, @Hide)";
        var dataQuery = await _postReportRepository.Connection.QueryAsync<RelatedBoxQueryResponse>(query, new
        {
            Limit = input.PageSize,
            Hide = input.Hides
        });
        var items = MappingRelatedBoxResponse(dataQuery);

        if (items.Any())
        {
            var results = new PagedResponse<RelatedBoxResponse>(0, input.PageNumber, input.PageSize);
            results.Items = items;
            return results;
        }
        else
        {
            return new PagedResponse<RelatedBoxResponse>(0);
        }
    }


    private List<RelatedBoxResponse> MappingRelatedBoxResponse(IEnumerable<RelatedBoxQueryResponse> posts)
    {
        return posts.Select(x => new RelatedBoxResponse
        {
            Title = x.Title,
            HashId = x.HashId,
            ThumbnailUrl = x.ThumbnailUrl,
            Id = x.Id,
            Tags = x.Tags,
            TotalComment = x.TotalComment,
            Reaction = new ReactionsResponse
            {
                TotalReacts = x.TotalReacts,
                Reactions = x.ReactionStr != null ? JsonConvert.DeserializeObject<List<ReactionResponse>>(x.ReactionStr) : new List<ReactionResponse>()
            }
        }).ToList();
    }

    public async Task<List<PostBoxResponse>> GetPostDetails(string hashIds)
    {
        var param = new { HashIds = hashIds.Split(',').ToList() };
        var result = await _postRepository.Connection.QueryAsync<PostBoxQueryResponse>(GetPostDetailsQuery, param);
        var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;

        if (result != null && result.Any())
        {
            var listPostDetails = new List<PostBoxResponse>();

            foreach (var res in result)
            {
                var chapters = res.SubPosts != null ? JsonConvert.DeserializeObject<List<SubPostDto>>(res.SubPosts.ToString()) : new List<SubPostDto>();
                if (chapters != null && chapters.Count > 0)
                {
                    chapters.Reverse();
                }
                var postDetails = new PostBoxResponse
                {
                    Id = res.Id,
                    IsMature = res.IsMature,
                    IsCurrentUserAuthor = res.UserId == currentUserId,
                    ThumbnailUrl = res.ThumbnailUrl,
                    Body = res.Body,
                    Title = res.Title,
                    AuthorId = res.AuthorId,
                    AuthorName = res.AuthorName,
                    ViewCount = res.ViewCount,
                    Tags = res.Tags != null ? JsonConvert.DeserializeObject<List<string>>(res.Tags.ToString()) : new List<string>(),
                    Chapters = chapters,
                    ChapterCount = res.ChapterCount,
                    Type = res.Type,
                    HashId = res.HashId,
                    CreatedOn = res.CreatedOn,

                };

                listPostDetails.Add(postDetails);
            }

            return listPostDetails;
        }
        else
        {
            return new List<PostBoxResponse>(); // Trả về danh sách rỗng nếu không có kết quả
        }
    }

    public UploadFileDto MappingFile(SocialResource resources)
    {
        if (resources == null || resources.Id == Guid.Empty)
        {
            return null;
        }
        return new UploadFileDto
        {
            HashId = resources.HashId,
            Order = resources.Order,
            Name = resources.Name,
            Url = _sc.GetPublicUrl(resources.Url, resources.Url, resources.MinioInstance).GetAwaiter().GetResult(),
            Height = resources.Height,
            Width = resources.Width,
            Type = resources.Type,
            Status = resources.Status,
            Size = resources.Size
        };
    }
    private List<PostSeriesTopResponse> MappingTopSeries(IEnumerable<PostSeriesTopQueryDbResponse> posts)
    {
        return posts.Select(x => new PostSeriesTopResponse
        {
            ProfileId = x.ProfileId,
            ProfileName = x.ProfileName,
            UserName = x.UserName,
            UserAvatar = x.UserAvatar,
            Title = x.Title,
            ViewCount = x.ViewCount ?? 0,
            CommentCount = x.CommentCount ?? 0 + x.TotalSubPostComment,
            ChapterCount = x.ChapterCount,
            Body = System.Web.HttpUtility.HtmlDecode(x.Body),
            Tags = x.Tags,
            Type = x.Type,
            AuthorName = x.AuthorName,
            AuthorId = x.AuthorId,
            CoverUrl = x.CoverUrl,
            ThumbnailUrl = x.ThumbnailUrl,
            CreatedOn = x.CreatedOn,
            Id = x.Id,
            IsMature = x.IsMature,
            IsCompleted = x.IsCompleted,
            Permission = x.Permission,
            Status = x.Status,
            UserId = x.UserId,
            //"AuthorName", "CoverUrl","CreatedOn", "IsMature", "Id", "Permission", "Status", "UserId"
            HashId = x.HashId,
            Chapters = MappingTopChapter(x.SubPostStr),
        }).ToList();
    }

    private List<PostBoxResposne> MappingToPostBoxResponse(IEnumerable<PostSeriesTopQueryDbResponse> posts)
    {
        return posts.Select(x => new PostBoxResposne
        {
            ProfileName = x.ProfileName,
            UserName = x.UserName,
            Title = x.Title,
            CommentCount = x.CommentCount ?? 0 + x.TotalSubPostComment,
            Body = System.Web.HttpUtility.HtmlDecode(x.Body),
            Tags = x.Tags,
            ThumbnailUrl = x.ThumbnailUrl,
            Id = x.Id,
            IsMature = x.IsMature,
            HashId = x.HashId,
            Type = x.Type,
            Chapters = MappingTopChapter(x.SubPostStr),
        }).ToList();
    }

    private List<PostSeriesTopResponse> MapTopSeries(IEnumerable<PostSeriesTopQueryDbResponse> posts)
    {
        return posts.Select(x => new PostSeriesTopResponse
        {
            ProfileId = x.ProfileId,
            ProfileName = x.ProfileName,
            UserName = x.UserName,
            UserAvatar = x.UserAvatar,
            Title = x.Title,
            ViewCount = x.ViewCount ?? 0,
            CommentCount = x.CommentCount ?? 0 + x.TotalSubPostComment,
            ChapterCount = x.ChapterCount,
            Body = System.Web.HttpUtility.HtmlDecode(x.Body),
            Tags = x.Tags,
            Type = x.Type,
            AuthorName = x.AuthorName,
            AuthorId = x.AuthorId,
            CoverUrl = x.CoverUrl,
            ThumbnailUrl = x.ThumbnailUrl,
            CreatedOn = x.CreatedOn,
            Id = x.Id,
            IsMature = x.IsMature,
            IsCompleted = x.IsCompleted,
            Permission = x.Permission,
            Status = x.Status,
            UserId = x.UserId,
            HashId = x.HashId,
            Chapters = MappingTopChapter(x.SubPostStr),
            SeriesStatus = x.ToSeriesStatus(),
            TotalComment = x.TotalComment,
            Reaction = new ReactionsResponse
            {
                TotalReacts = x.TotalReact,
                Reactions = x.ReactionByPostStr != null ? JsonConvert.DeserializeObject<List<ReactionResponse>>(x.ReactionByPostStr) : new List<ReactionResponse>()
            }
        }).ToList();
    }

    private string GetQuerySelectPage(PostSeriesSelectedType selectedType)
    {
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;
        switch (selectedType)
        {
            case PostSeriesSelectedType.HIT:
                {
                    topSelectPostIdQuery = GetTopPostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopPostHitToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.LATEST:
                {
                    topSelectPostIdQuery = GetTopLatestPostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestPostToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.COMPLETED:
                {
                    topSelectPostIdQuery = GetTopLatestCompletePostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestCompletePostToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.BY_TAG:
                {
                    topSelectPostIdQuery = GetLatestPostByTagHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetLatestPostByTagToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.RECOMMEND:
                {
                    topSelectPostIdQuery = GetTopRecommendedPostHitQuery;
                    countTopQuery = "";
                    break;
                }
            case PostSeriesSelectedType.BY_USER:
                {
                    topSelectPostIdQuery = GetLatestPostByUserHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetLatestPostByUserToCountQuery);
                    break;
                }
            default:
                {
                    break;
                }
        }


        var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", topSelectPostIdQuery)
            .Replace("[CountResults]", countTopQuery)
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
            .Replace("[OrderBy]", "CreatedOn");

        return query;
    }

    private bool CheckIsPublicNow(DateTime? PublishDate)
    {
        return (PublishDate != null && PublishDate < DateTime.UtcNow);
    }

    #endregion

    #region Chapters
    public async Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq)
    {
        PagedResponse<ChapterTOCExtendResponse> results;
        var query = GetSeriesChaptersWithOffsetSimpleByHashId;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(SocialSubPost.Order);
        }

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    UserId = userId,
                    PostHashId = hashId,
                    PageSize = loadReq.PageSize,
                    Offet = offset
                });
        var items = await multi.ReadAsync<ChapterTOCExtendResponse>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            results = new PagedResponse<ChapterTOCExtendResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
            results.Items = items;
        }
        else
        {
            results = new PagedResponse<ChapterTOCExtendResponse>(0);
        }
        return results;
    }

    private List<ChapterBasicResponse> MappingTopChapter(string subPostStr)
    {
        var listChapter = (JsonConvert.DeserializeObject<List<ChapterBasicResponse>>(subPostStr))?.Where(x => x != null).
            OrderByDescending(x => x.Order).ToList();

        listChapter.ForEach(x =>
        {
            x.IsPublicNow = true;
            if (x.ViewCount == null) { x.ViewCount = 0; }
        });

        return listChapter;
    }
    #endregion

    private int GetOffsetSetup(ref ComicTopPostR loadReq)
    {
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(SocialPost.CreatedOn);
        }
        return offset;
    }

    private void ValidateTotalItem(int number)
    {
        if (number > PostConfig.ItemCountMax)
        {
            throw new BadRequestException(ApiErrorCode.POST_REQ_TOTAL_EXCEEDS_LIMIT, ApiErrorMessage.POST_REQ_TOTAL_EXCEEDS_LIMIT);
        }
    }

    private string AddWithPermission(string query, Guid? currentUserId)
    {
        //TODO Premium
        string withPermission = (currentUserId == null ? @" AND (sp.""Permission"" = 0 OR sp.""Permission"" = 2) " : @" AND ((sp.""Permission"" = 0 OR sp.""Permission"" = 2) OR ( sp.""UserId"" = @UserId )) ");
        query = query.Replace("[WithPermission]", withPermission);
        return query;
    }

    public async Task<List<RewardDto>> CheckRewardsForPost(Guid userId, PostType type)
    {
        var res = new List<RewardDto>();

        var check = await _context.SocialPostAvailable.FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
        if (check == null)
        {
            var rewardType = RewardType.FirstFeed;
            switch (type)
            {
                case PostType.Story:
                    rewardType = RewardType.FirstStory;
                    break;

                case PostType.Comic:
                    rewardType = RewardType.FirstComic;
                    break;

                default:
                    break;
            }

            res.Add(new RewardDto
            {
                Type = rewardType,
                MessageCode = rewardType.ToString()
            });
        }

        return res;
    }

    public async Task<IEnumerable<string>> GetSubPostRandomIdsAsync(PostRandomIdsR input)
    {
        try
        {
            var query = $@"SELECT sp.""HashId""
                               FROM social.""SocialSubPosts"" sp
                               JOIN social.""SocialPosts"" p ON sp.""PostId"" = p.""Id""
                               JOIN social.""SocialResources"" r on sp.""Id"" = r.""SubPostId""
                               WHERE p.""IsDelete"" = false
                               AND sp.""IsDelete"" = false
                               [QueryByType]
                               [IgnoreQuery]
                               AND sp.""Order"" = (
                                                    SELECT MIN(sp_inner.""Order"")
                                                    FROM social.""SocialSubPosts"" sp_inner
                                                    WHERE sp_inner.""PostId"" = sp.""PostId""
                                                    AND sp_inner.""IsDelete"" = false
                                                    )
                               ORDER BY RANDOM()
                               LIMIT @PageSize";
            query = query.Replace("[QueryByType]", input.IsGetAllType ? "" : $@"AND p.""Type"" = {(int)PostType.Feed}");
            query = query.Replace("[IgnoreQuery]", input.PostRandomIds == null ? "" : $@"AND NOT sp.""HashId"" = ANY(@PostRandomIds)");
            return await _postReportRepository.Connection.QueryAsync<string>(query, new
            {
                PostRandomIds = input.PostRandomIds?.ToList(),
                PageSize = input.AmountItem
            });

        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR input)
    {
        try
        {
            var query = @$"SELECT ""HashId"" From social.""SocialPosts"" 
                                WHERE ""IsDelete"" = false
                                [QueryByType]
                                [IgnoreQuery]
                                ORDER BY RANDOM()
                                LIMIT @PageSize";
            query = query.Replace("[QueryByType]", input.IsGetAllType ? "" : $@"AND ""Type"" = {(int)PostType.Feed}");
            query = query.Replace("[IgnoreQuery]", input.PostRandomIds == null ? "" : $@"AND NOT ""HashId"" = ANY(@PostRandomIds)");
            return await _postReportRepository.Connection.QueryAsync<string>(query, new
            {
                PostRandomIds = input.PostRandomIds?.ToList(),
                PageSize = input.AmountItem
            });
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<Tuple<int, int>> GetFollowedPostCount(BaseR req)
    {
        var currentUserId = _currentUserService.Session?.UserId;
        var followedComicCount = await (
            from qpost in _context.ComicPosts.AsNoTracking()
            join cfp in _context.ComicPostFavoriteAvailable.AsNoTracking()
                on qpost.Id equals cfp.PostId
            where cfp.UserId == currentUserId
                  && !cfp.IsDelete
                  && !qpost.IsDelete
                  && (!req.Hides.Contains((int)qpost.Hide))
            select qpost
        ).CountAsync();

        var followedStoryCount = await (
            from qpost in _context.StoryPosts.AsNoTracking()
            join cfp in _context.StoryPostFavoriteAvailable.AsNoTracking()
                on qpost.Id equals cfp.PostId
            where cfp.UserId == currentUserId
                  && !cfp.IsDelete
                  && !qpost.IsDelete
                  && (!req.Hides.Contains((int)qpost.Hide))
            select qpost
        ).CountAsync();
        return Tuple.Create(followedComicCount, followedStoryCount);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialPostComment> _postCommentRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IRepository<SocialPostReport> _postReportRepository;

    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISmartLookupService _smartLookupService;

    #endregion
}
