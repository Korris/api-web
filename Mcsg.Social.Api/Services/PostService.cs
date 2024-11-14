using AutoMapper;
using Dapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Social.Api.Services;

using Analytic.Application.Protos;
using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Constants;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Models;
using Models.Earning;
using Requests;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class PostService : BaseMinioS, IPostService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="unitOfWork"></param>
    /// <param name="mapper"></param>
    /// <param name="smartLookupService"></param>
    public PostService(IMcsgContext context, ISetting setting, IStorageClient sc, IUnitOfWork unitOfWork, IMapper mapper, ISmartLookupService smartLookupService, IBusinessText businessText) : base(context, setting, sc)
    {
        _businessText = businessText;

        _unitOfWork = unitOfWork;
        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _smartLookupRepository = unitOfWork.GetRepository<SmartLookup>();
        _mapper = mapper;
        _smartLookupService = smartLookupService;
        _postCommentRepository = unitOfWork.GetRepository<SocialPostComment>();
    }

    public async Task<bool> Delete(IdBaseR request)
    {
        var postId = request.Id;
        var userId = request.UserId;
        var profileName = request.ProfileName;

        var feedDb = await _postRepository.GetByIdAsync(postId);
        if (feedDb == null)
        {
            throw new BadRequestException(E204, M204);
        }
        else if (feedDb.UserId != userId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = DateTime.UtcNow, UserId = userId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(postId, profileName);

            _ = Task.Run(async () => await SyncDeleteToAna(postId));

            return true;
        }
    }

    #region -- Series --
    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, PostTopR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(PostSeriesSelectedType.ByTag);

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

    public async Task<List<MyPostSeriesResponse>> GetMyAllSeries(BaseR request)
    {
        try
        {
            var userId = request.UserId;
            var query = string.Format(GetMyAllQuery);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        UserId = userId
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
        var query = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",pc.""CreatedOn"",p.""Type"" , 
                    pc.""GifId"",
                    sr.""Url"" as ResourceUrl,
                    sr.""MinioInstance"",
                    p.""HashId"" as HashPostId,
                    FALSE as IsSubPost , 
                    NULL as Order
                    from ""social"".""SocialPostComments"" pc 
                    left join ""social"".""SocialPosts"" p on  pc.""PostId"" = p.""Id""
                    LEFT JOIN ""social"".""SocialResources"" sr on pc.""ResourceId"" = sr.""Id""
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
            FromDate = DateTime.Today,
            UserIds = userFollowingIds,
            CurrentUserId = user.Id
        });
        var amountDataNeedToTake = data != null ? input.PageSize - data.Count() : input.PageSize;
        var queryDataNeedToTake = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",pc.""CreatedOn"", pc.""GifId"", p.""Type"" ,
                    sr.""Url"" as ResourceUrl,
                    sr.""MinioInstance"",
                        p.""HashId"" as HashPostId,
                        FALSE as IsSubPost, NULL as Order,
                        COALESCE(COUNT(pcr.""Id""), 0) AS reaction_count,
                         RANDOM() AS sort_key
                        FROM ""social"".""SocialPostComments"" pc
                        LEFT JOIN ""social"".""SocialResources"" sr on pc.""ResourceId"" = sr.""Id""
                        LEFT JOIN ""social"".""SocialPosts"" p on  pc.""PostId"" = p.""Id""
                        LEFT JOIN ""social"".""SocialPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
                        LEFT JOIN ""identity"".""Users"" u on pc.""CreatedBy"" = u.""Id""
                        WHERE pc.""Id"" <> ALL (ARRAY[@CommentIds]) 
                        AND pc.""CreatedBy"" != @CurrentUserId
                        AND p.""IsDelete"" = false
                        AND pc.""IsDelete"" = false
                        AND pc.""CreatedOn"" > @FromDate
                        GROUP BY p.""HashId"", u.""Avatar"",u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",p.""Type"",sr.""Url"",sr.""MinioInstance""

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

        var result = data.Concat(dataNeedToTake).ToList();
        var body = "";
        foreach (var item in result)
        {
            body += item.Body + " ";
        }
        var profiles = await _businessText.GetProfiles(body);

        var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialPostCommentReactions""");
        var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
        {
            TargetIds = result.Select(p => p.Id).ToList(),
            UserId = input.UserId
        });

        foreach (var item in result)
        {
            item.ResourceUrl = await _sc.GetPublicUrl(item.ResourceUrl, item.BucketName, item.MinioInstance);
            item.Body = await _businessText.Process(item.Body, profiles);
            var postCommentReaction = postCommentReactionResponse.Where(p => p.TargetId == item.Id).ToList();
            if (postCommentReaction.Count > 0)
            {
                MapReactionNewsFeedResponse(item, postCommentReaction);
            }
        }
        return result;
    }

    private void MapReactionNewsFeedResponse(NewsFeedDto item, List<CommentReactionResponseQuery> reactions)
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

    public async Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input)
    {
        var query = "SELECT * FROM social.fn_get_visible_post_maybe_you_like(@Limit, @Hide)";
        var dataQuery = await _postRepository.Connection.QueryAsync<RelatedBoxQueryResponse>(query, new
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

    public async Task<List<PostBoxResponse>> GetPostDetails(string hashIds, BaseR request)
    {
        var param = new { HashIds = hashIds.Split(',').ToList() };
        var result = await _postRepository.Connection.QueryAsync<PostBoxQueryResponse>(GetPostDetailsQuery, param);
        var userId = request.UserId;

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
                    IsCurrentUserAuthor = res.UserId == userId,
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
            return new List<PostBoxResponse>();
        }
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
            Body = HttpUtility.HtmlDecode(x.Body),
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
            Body = HttpUtility.HtmlDecode(x.Body),
            Tags = x.Tags,
            ThumbnailUrl = x.ThumbnailUrl,
            Id = x.Id,
            IsMature = x.IsMature,
            HashId = x.HashId,
            Type = x.Type,
            Chapters = MappingTopChapter(x.SubPostStr),
        }).ToList();
    }

    private string GetQuerySelectPage(PostSeriesSelectedType selectedType)
    {
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;
        switch (selectedType)
        {
            case PostSeriesSelectedType.Hit:
                {
                    topSelectPostIdQuery = GetTopPostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopPostHitToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.Latest:
                {
                    topSelectPostIdQuery = GetTopLatestPostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestPostToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.Completed:
                {
                    topSelectPostIdQuery = GetTopLatestCompletePostHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestCompletePostToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.ByTag:
                {
                    topSelectPostIdQuery = GetLatestPostByTagHitQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetLatestPostByTagToCountQuery);
                    break;
                }
            case PostSeriesSelectedType.Recommend:
                {
                    topSelectPostIdQuery = GetTopRecommendedPostHitQuery;
                    countTopQuery = "";
                    break;
                }
            case PostSeriesSelectedType.ByUser:
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
    #endregion

    #region -- Chapters --
    public async Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, PostChapterListR loadReq)
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

    private int GetOffsetSetup(ref PostTopR loadReq)
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
            var results = new List<string>();
            int daysToCheck = 0; // Number of days to check
            DateTime? targetDate = null;

            while (results.Count < input.AmountItem) // Limit check to 30 days
            {
                var query = $@"SELECT sp.""HashId""
                        FROM ""social"".""SocialSubPosts"" sp
                        JOIN ""social"".""SocialPosts"" p ON sp.""PostId"" = p.""Id""
                        JOIN ""social"".""SocialResources"" r ON sp.""Id"" = r.""SubPostId""
                        WHERE p.""IsDelete"" = false
                        AND sp.""IsDelete"" = false
                        AND p.""CreatedOn""::date = @TargetDate
                        [QueryByType]
                        [IgnoreQuery]
                        AND sp.""Order"" = (
                                             SELECT MIN(sp_inner.""Order"")
                                             FROM ""social"".""SocialSubPosts"" sp_inner
                                             WHERE sp_inner.""PostId"" = sp.""PostId""
                                             AND sp_inner.""IsDelete"" = false
                                             )
                        ORDER BY RANDOM()
                        LIMIT @PageSize";

                if (targetDate.HasValue)
                {
                    targetDate = targetDate.Value.Date.AddDays(-1);
                }
                else
                {
                    var lastHashId = input.PostRandomIds?.LastOrDefault();
                    var isDataFromSubPost = await _context.SocialSubPostAvailable.AnyAsync(p => p.HashId == lastHashId);
                    var tableName = isDataFromSubPost ? @"social.""SocialSubPosts""" : @"social.""SocialPosts""";
                    var createdOnQuery = $@"SELECT sp.""CreatedOn""
                                            FROM {tableName} sp 
                                            WHERE sp.""HashId"" = @LastHashId";

                    targetDate = await _postRepository.Connection.QuerySingleOrDefaultAsync<DateTime?>(createdOnQuery, new
                    {
                        LastHashId = lastHashId
                    });
                }

                query = query.Replace("[QueryByType]", input.IsGetAllType ? "" : $@"AND p.""Type"" = {(int)PostType.Feed}");
                query = query.Replace("[IgnoreQuery]", input.PostRandomIds == null ? "" : $@"AND NOT sp.""HashId"" = ANY(@PostRandomIds)");

                var subPostIds = await _postRepository.Connection.QueryAsync<string>(query, new
                {
                    PostRandomIds = input.PostRandomIds?.ToList(),
                    PageSize = input.AmountItem - results.Count, // Get the remaining amount needed
                    TargetDate = targetDate.Value // Ensure a non-null value is used
                });

                results.AddRange(subPostIds); // Add new results to the list
            }
            return results.Take(input.AmountItem); // Return the required amount
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
            var query = @$"SELECT ""HashId"" From ""social"".""SocialPosts"" 
                                WHERE ""IsDelete"" = false
                                [QueryByType]
                                [IgnoreQuery]
                                ORDER BY RANDOM()
                                LIMIT @PageSize";
            query = query.Replace("[QueryByType]", input.IsGetAllType ? "" : $@"AND ""Type"" = {(int)PostType.Feed}");
            query = query.Replace("[IgnoreQuery]", input.PostRandomIds == null ? "" : $@"AND NOT ""HashId"" = ANY(@PostRandomIds)");
            return await _postRepository.Connection.QueryAsync<string>(query, new
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
        var userId = req.UserId;
        var followedComicCount = await (
            from a in _context.ComicPostAvailable.AsNoTracking()
            join b in _context.ComicPostFavoriteAvailable.AsNoTracking()
                on a.Id equals b.PostId
            where b.UserId == userId
                  && !a.IsDelete
                  && (!req.Hides.Contains((int)a.Hide))
            select a
        ).CountAsync();

        var followedStoryCount = await (
            from a in _context.StoryPosts.AsNoTracking()
            join b in _context.StoryPostFavoriteAvailable.AsNoTracking()
                on a.Id equals b.PostId
            where b.UserId == userId
                  && !b.IsDelete
                  && !a.IsDelete
                  && (!req.Hides.Contains((int)a.Hide))
            select a
        ).CountAsync();
        return Tuple.Create(followedComicCount, followedStoryCount);
    }

    #region -- Post --
    private async Task<SocialDeleteRsp> SyncDeleteToAna(Guid id)
    {
        var res = new SocialDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);
            var client = new SocialProto.SocialProtoClient(channel);

            var request = new SocialDeleteReq
            {
                PostId = id.ToString()
            };
            var rsp = await client.DeleteAsync(request);

            res.Message = rsp.Message;
            res.Id = rsp.Id;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }
    #endregion

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business Text
    /// </summary>
    private readonly IBusinessText _businessText;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialPostComment> _postCommentRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IMapper _mapper;
    private readonly ISmartLookupService _smartLookupService;

    #endregion
}
