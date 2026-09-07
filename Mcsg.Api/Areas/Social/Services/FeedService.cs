using AutoMapper;
using Dapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Web;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Social.Services;

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
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Extensions;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Error;

public partial class FeedService : IFeedService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="businessBodyText"></param>
    /// <param name="postService"></param>
    /// <param name="soundService"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="mapper"></param>
    /// <param name="feedDisplayConfig"></param>
    public FeedService(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessBodyText, IPostService postService, ISoundService soundService, IUnitOfWork unitOfWork, IMapper mapper, IOptionsMonitor<FeedDisplayConfig> feedDisplayConfig)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _businessText = businessBodyText;
        _postService = postService;
        _soundService = soundService;

        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _mapper = mapper;
        _feedDisplayConfig = feedDisplayConfig.CurrentValue;
    }

    public async Task<List<SharePostResponse>> GetSharePosts(BaseR req, List<SharePostInput>? ids)
    {
        var res = new List<SharePostResponse>();

        if (ids == null || ids.Count == 0)
        {
            return res;
        }

        var schema = "social";
        var fn = "social.fw_share_post_by_list_ids";
        var @params = "@PostInfoJson, @Hide";

        var postInfoArray = ids.Select(item => new
        {
            id = item.Id.ToString(),
            type = (int)item.Type
        }).ToList();

        var postInfoJson = System.Text.Json.JsonSerializer.Serialize(postInfoArray);
        var paramValues = new
        {
            PostInfoJson = postInfoJson,
            Hide = req.Hides,
        };

        var connection = _context.Database.GetDbConnection();
        var result = await connection.QueryAsync<ShareBoxQueryResponse>(fn.ToFn("social", schema, @params), paramValues);
        if (result == null || !result.Any())
        {
            return res;
        }

        var body = "";
        foreach (var i in result)
        {
            body += i.Body + " ";
        }
        var profiles = await _businessText.GetProfiles(body);

        foreach (var i in result)
        {
            i.Body = await _businessText.Process(i.Body, profiles);
            res.Add(MappingSharePostResponse(i));
        }

        var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
        var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""),
            new
            {
                TargetIds = res.Select(p => p.Id).ToList(),
                req.UserId
            });

        if (postReactionResponse.Any())
        {
            foreach (var item in res)
            {
                var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionSharePostResponse(item, postReaction);
                }
            }
        }

        return res;
    }

    public async Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);
            var date = DateTime.UtcNow.Date;
            var frDate = date.StartOfDayUtc();
            var toDate = date.EndOfDayUtc();
            var queryPostIds = new List<Guid>();
            var listPostTracking = new TrackingSummaryGetFeedsRsp();
            IEnumerable<FeedsListQueryDbDto> items;
            var totalItems = 0;
            var userId = feedLoadReq.UserId;

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(SocialPost.CreatedOn);
            }
            var query = "";
            if (loadFeedType == LoadFeedType.Trending || loadFeedType == LoadFeedType.Hot)
            {
                query = string.Format(GetAllFeedsWithTopCommentQuery, _postRepository.TableName, feedLoadReq.OrderBy);
                if (loadFeedType == LoadFeedType.Hot)
                {
                    int offsetDate = date.DayOfWeek - DayOfWeek.Monday;
                    DateTime lastMonday = date.AddDays(-offsetDate);
                    date = lastMonday;
                }
            }

            var isMySelf = feedLoadReq.NewUserName == feedLoadReq.UserName;
            if (feedLoadReq.NewUserName != null)
            {
                query = string.Format(GetAllFeedsQuery, _postRepository.TableName, feedLoadReq.OrderBy);
                var newUserNameQuery = $@"OR (u.""UserName"" = @NewUserName AND @MySelf)";
                query = query.Replace("[AddNewUserNameContidion]", newUserNameQuery);

                query = AddAdditionalFeedQuery(feedLoadReq, query, loadFeedType);

                var multi = await _postRepository
                        .Connection.QueryMultipleAsync(query, new
                        {
                            Type = (int)PostType.Feed,
                            IsAccessPrivate = false,
                            feedLoadReq.PageSize,
                            Offet = offset,
                            Date = date,
                            PostStatus = StatusUtils.PostStatusIntPublic,
                            DateOnly = DateOnly.FromDateTime(date),
                            Hide = feedLoadReq.Hides,
                            MySelf = isMySelf,
                            feedLoadReq.NewUserName,
                        });
                items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
                totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            }
            else
            {
                var postHideIds = await _context.Available<SocialPostHide>(false).Where(p => p.UserId == userId).Select(p => p.PostId.ToString()).ToListAsync();
                listPostTracking = await GetFeeds(feedLoadReq.PageNumber, feedLoadReq.PageSize, frDate.ToString(), toDate.ToString(), feedLoadReq.UserId, string.Join(",", postHideIds));

                Console.WriteLine($"[FEED ANALYTIC] gRPC Success={listPostTracking.Success}, Items={listPostTracking.Items.Count}, TotalRecords={listPostTracking.TotalRecords}, Message={listPostTracking.Message}");

                // Fallback: query directly from DB when Analytic gRPC returns no data
                if (listPostTracking.Items.Count == 0)
                {
                    Console.WriteLine("[FEED FALLBACK] Analytic returned no data, querying DB directly");
                    query = string.Format(GetAllFeedsQuery, _postRepository.TableName, feedLoadReq.OrderBy ?? nameof(SocialPost.CreatedOn));
                    query = query.Replace("[AddNewUserNameContidion]", "")
                                .Replace("[AdditionalCondition]", "")
                                .Replace("[AdditionalTotalQuery]", "")
                                .Replace("[AdditionalTotalCondition]", "");

                    var multi = await _postRepository
                            .Connection.QueryMultipleAsync(query, new
                            {
                                Type = (int)PostType.Feed,
                                IsAccessPrivate = false,
                                feedLoadReq.PageSize,
                                Offet = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1),
                                Date = date,
                                PostStatus = StatusUtils.PostStatusIntPublic,
                                DateOnly = DateOnly.FromDateTime(date),
                                Hide = feedLoadReq.Hides,
                                MySelf = isMySelf,
                                feedLoadReq.NewUserName,
                            });
                    items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
                    totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
                    Console.WriteLine($"[FEED FALLBACK] DB returned {items.Count()} items, total={totalItems}");
                }
                else
                {
                    queryPostIds = listPostTracking.Items.Select(x => Guid.Parse(x.PostId)).ToList();
                    totalItems = listPostTracking.TotalRecords;
                    var param = new
                    {
                        PostIds = queryPostIds,
                        PostStatus = StatusUtils.PostStatusIntPublic,
                        Type = (int)PostType.Feed,
                        MySelf = isMySelf,
                    };

                    query = "SELECT * FROM social.fw_get_posts_by_postid_trackings(@PostIds, @Type, @MySelf, @PostStatus)";

                    var connection = _context.Database.GetDbConnection();
                    try
                    {
                        var tempResults = await connection.QueryAsync<FeedsListQueryDbDto>(query, param);
                        items = [.. tempResults.OrderBy(p => queryPostIds.IndexOf(p.Id))];
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }

            var listItemResponse = new List<FeedDto>();

            var postIds = await _context.Available<SocialPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId).ToListAsync();

            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""),
                new
                {
                    TargetIds = items.Select(p => p.Id).ToList(),
                    UserId = userId
                });

            var sharePostIds = items.Where(p => p.SharePostId.HasValue)
                .Select(p => new SharePostInput
                {
                    Id = p.SharePostId!.Value,
                    Type = p.SharePostType.HasValue ? p.SharePostType.Value : SharePostType.Feed
                })
                .ToList();
            var sharePosts = await GetSharePosts(feedLoadReq, sharePostIds);

            var commentsResult = await GetMostCommentReaction(items.Select(p => p.HashId).ToList());
            var comments = await GetReactionAndMentionOfComment(commentsResult.Comments, userId);

            foreach (var item in items)
            {
                var feed = MappingFeedInListRespone(item, postIds, isMySelf);

                feed.Body = await _businessText.Process(feed.Body, profiles);
                feed.IsCensored = !feedLoadReq.IsAdministrator && feedLoadReq.UserName != feed.UserName && feed.Status == PostStatus.Inactive;
                feed.IsBlur = feed.Status == PostStatus.Inactive;

                var postReaction = postReactionResponse.Where(p => p.TargetId == feed.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionFeedDtoResponse(feed, postReaction);
                }

                var sharePost = sharePosts.FirstOrDefault(p => p.Id == feed.SharePostId);
                if (sharePost != null)
                {
                    feed.SharePost = sharePost;
                }

                var totalComments = commentsResult.TotalComment?.FirstOrDefault(p => p.PostHashId == feed.HashId)?.TotalCommentCount;
                feed.Comments = new CommentPagedResults<MostReactionCommentResponse>(totalComments ?? 0, 1, 2)
                {
                    Items = comments.Where(p => p.PostHashId == feed.HashId),
                    TotalComments = totalComments ?? 0
                };

                listItemResponse.Add(feed);
            }

            results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
            results.Items = listItemResponse;

            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FEED ERROR] {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"[FEED ERROR STACK] {ex.StackTrace}");
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<PagedResponse<FeedDto>> GetFeedByUserNameOrKeyword(FeedPostByProFileNameR feedLoadReq)
    {
        PagedResponse<FeedDto> results;
        var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

        var queryCondition = "";

        if (feedLoadReq.SearchBy == "ProfileName")
        {
            queryCondition = $@"LEFT JOIN identity.""Users"" u
                                ON qpost.""UserId"" = u.""Id""
                                WHERE u.""ProfileName""=@ProfileName 
                                   AND u.""IsDelete"" = false
                                   AND qpost.""Type""=@PostType
                                   AND qpost.""Status"" = ANY (@PostStatus)
                                   AND qpost.""IsDelete""=false";
        }
        else
        {
            queryCondition = $@" WHERE unaccent(qpost.""Body"") ILIKE unaccent('%{feedLoadReq.Keyword}%')
                                     AND qpost.""Type""=@PostType
                                     AND qpost.""Status"" = ANY (@PostStatus)
                                     AND qpost.""IsDelete""=false";
        }

        if (feedLoadReq.OrderBy == null)
        {
            feedLoadReq.OrderBy = nameof(SocialPost.CreatedOn);
        }

        var query = string.Format(GetAllFeedsByCondition, _postRepository.TableName, feedLoadReq.OrderBy);
        query = query.Replace("[QueryCondition]", queryCondition);
        var multi = await _postRepository
                   .Connection.QueryMultipleAsync(query, new
                   {
                       PostType = (int)PostType.Feed,
                       IsAccessPrivate = false,
                       PageSize = feedLoadReq.PageSize,
                       Offet = offset,
                       ProfileName = feedLoadReq.Keyword,
                       PostStatus = StatusUtils.PostStatusIntPublic,
                       Hide = feedLoadReq.Hides
                   });
        var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
        var listItemResponse = new List<FeedDto>();

        var body = "";

        foreach (var i in items)
        {
            body += i.Body + " ";
        }
        var profiles = await _businessText.GetProfiles(body);

        foreach (var item in items)
        {
            item.Body = await _businessText.Process(item.Body, profiles);
            listItemResponse.Add(MappingFeedInListRespone(item, null, null));
        }
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        foreach (var item in listItemResponse)
        {
            item.IsCensored = !feedLoadReq.IsAdministrator && feedLoadReq.UserName != item.UserName && item.Status == PostStatus.Inactive;
            item.IsBlur = item.Status == PostStatus.Inactive;
        }

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
            results.Items = listItemResponse;

            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                feedLoadReq.UserId
            });

            foreach (var item in results.Items)
            {
                var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapPostBoxReactionResponse(item, postReaction);
                }
            }
        }
        else
        {
            results = new PagedResponse<FeedDto>(0);
        }
        return results;
    }

    public async Task<PagedResponse<FeedDto>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(SocialPost.CreatedOn);
            }
            var query = string.Format(GetAllFeedsByTagQuery, _postRepository.TableName, feedLoadReq.OrderBy);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        PageSize = feedLoadReq.PageSize,
                        Offet = offset,
                        TagName = tagName
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();

            var body = "";

            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            foreach (var item in items)
            {
                item.Body = await _businessText.Process(item.Body, profiles);
                listItemResponse.Add(MappingFeedInListRespone(item, null, null));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<SubPostFeedResponse> GetFeedSubPostAsync(IdBaseR request)
    {
        var hashId = request.HashId;
        var userId = request.UserId;

        var query = $@"WITH SubPostsCount AS (
                         SELECT ""PostId"", 
                                COUNT(*) AS total_subposts 
                         FROM social.""SocialSubPosts""
                         WHERE ""IsDelete"" = false
                         GROUP BY ""PostId""
                     ),
                     ResourceCount AS (
                         SELECT sp.""PostId"",
                         to_jsonb(array_agg(
                         json_build_object(
                             'Url', r.""Url"",
                             'MinioInstance', r.""MinioInstance"",
                             'Height', r.""Height"",
                             'Width', r.""Width"",
                             'BucketName', r.""BucketName"",
                             'Type', r.""Type"",
                             'Name', r.""Name"",
                             'Order',r.""Order"",
                             'HashId',r.""HashId"",
                             'SubPostHashId',sp.""HashId"",
                             'Body', sp.""Body""
                         )
                         )) AS Resources
                         FROM social.""SocialSubPosts"" sp
                         LEFT JOIN social.""SocialResources"" r ON sp.""Id"" = r.""SubPostId""
                         WHERE sp.""IsDelete"" = false
                         GROUP BY sp.""PostId""
                     )
                     SELECT 
                         u.""ProfileName"" AS Fullname,
                         u.""ProfileId"",
                         u.""UserName"",
                         u.""Avatar"" AS UserAvatar,
                         u.""Id"" AS UserId,
                         p.""HashId"",
                         p.""Id"" AS ParentId,
                         sp.""HashId"" as SubPostHashId,
                         sp.""Id"",
                         sp.""CreatedOn"", 
                         sp.""Body"",
                         sp.""CreatedBy"",
                         p.""CustomNote"", 
                         COALESCE(psb.""HashId"", (
                             SELECT ps.""HashId"" 
                             FROM social.""SocialSubPosts"" ps 
                             WHERE ps.""PostId"" = sp.""PostId"" 
                               AND ps.""Order"" = sc.total_subposts
                               AND ps.""IsDelete"" = false
                         )) AS PrevSubPostHashId,
                         COALESCE(asp.""HashId"", (
                             SELECT ps.""HashId"" 
                             FROM social.""SocialSubPosts"" ps 
                             WHERE ps.""PostId"" = sp.""PostId"" 
                               AND ps.""Order"" = 1
                               AND ps.""IsDelete"" = false
                         )) AS NextSubPostHashId,
                         rc.Resources as ""ResourcesStr""
                     FROM social.""SocialSubPosts"" sp
                     LEFT JOIN identity.""Users"" u ON u.""Id"" = sp.""UserId""
                     LEFT JOIN social.""SocialPosts"" p ON p.""Id"" = sp.""PostId""
                     LEFT JOIN social.""SocialSubPosts"" psb ON sp.""PostId"" = psb.""PostId""
                      AND sp.""Order"" = psb.""Order"" + 1
                      AND psb.""IsDelete"" = false
                      LEFT JOIN social.""SocialSubPosts"" asp ON sp.""PostId"" = asp.""PostId""
                      AND sp.""Order"" = asp.""Order"" - 1
                      AND asp.""IsDelete"" = false
                      LEFT JOIN SubPostsCount sc ON sp.""PostId"" = sc.""PostId""
                      LEFT JOIN ResourceCount rc ON sp.""PostId"" = rc.""PostId""
                      WHERE sp.""HashId"" = @Id
                     AND sp.""IsDelete"" = false";

        var dataQuery = await _postRepository.Connection.QueryFirstOrDefaultAsync<SubPostFeedQuery>(query, new
        {
            Id = hashId
        });
        if (dataQuery == null)
        {
            throw new NotFoundException(nameof(E208), E208);
        }

        var resource = JsonConvert.DeserializeObject<List<ResourceDto>>(dataQuery?.ResourcesStr ?? "") ?? [];
        var data = _mapper.Map<SubPostFeedResponse>(dataQuery);
        data.Resources = resource;
        /// if only 1 Resource when click popup will show data of this Post instead of SubPost
        if (data.Resources.Count == 1)
        {
            var postData = await _postRepository.Connection.QueryFirstAsync<SubPostFeedResponse>($@"SELECT ""Body"",""Id"",""HashId"" from social.""SocialPosts"" WHERE ""HashId"" =@Id", new { Id = data.HashId });
            data.Id = postData.Id;
            data.Body = postData.Body;
            data.HashId = postData.HashId;
            data.Body = HttpUtility.HtmlDecode(data.Body);
        }

        data.Resources = data.Resources.OrderBy(p => p.Order).ToList();
        foreach (var item in data.Resources)
        {
            item.Url = await _sc.GetPublicUrl(item.Url, item.BucketName, item.MinioInstance);
        }

        data.SubPosts.Add(new SubUploadFileDto
        {
            Files = new List<UploadFileDto>()
             {
                 new UploadFileDto()
                 {
                     HashId = hashId,
                     Height = data.Height,
                     Width = data.Width,
                     Url = data.Url,
                     Type = data.ResourceType,
                     Name = data.ResourceName
                 }
             },
            Body = data.Body
        });

        var tableName = data.Resources.Count == 1 ? $@"social.""SocialPostReactions""" : $@"social.""SocialSubPostReactions""";
        var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
        var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, tableName), new
        {
            TargetIds = new List<Guid>() { data.Id },
            UserId = userId
        });

        if (postReactionResponse.Count() > 0)
        {
            MapReactionFeedDtoResponse(data, postReactionResponse.ToList());
        }
        data.IsFavorite = await _context.Available<SocialPostFavorite>().AnyAsync(p => p.UserId == userId && p.PostId == data.ParentId);
        data.Body = await _businessText.Process(data.Body);
        return data;
    }
    public async Task<FeedDto> GetFeedAsync(FeedHashIdR req)
    {
        var hashId = req.HashId;
        var userId = req.UserId;
        var query = string.Format(GetFeedQuery, _postRepository.TableName);

        FeedQueryDbDto? dbFeed = null;
        await _postRepository
            .Connection.QueryAsync<FeedQueryDbDto, SubPostQueryDbDto, UploadFileQueryDbDto, MetaDataQueryDto, PostLinkDbDto, FeedQueryDbDto>(query,
            (feed, subpost, uploadfiles, meta, link) =>
            {
                if (dbFeed == null)
                {
                    dbFeed = feed;
                }
                if (subpost != null)
                {
                    if (dbFeed.SubPostDbs == null)
                        dbFeed.SubPostDbs = new List<SubPostQueryDbDto>();
                    if (uploadfiles != null)
                    {
                        if (subpost.FileDbs == null)
                            subpost.FileDbs = new List<UploadFileQueryDbDto>();
                        subpost.FileDbs.Add(uploadfiles);
                    }
                    dbFeed.SubPostDbs.Add(subpost);
                }
                if (meta != null)
                {
                    if (feed.MetaDataDb == null)
                        feed.MetaDataDb = meta;
                }
                if (link != null)
                {
                    if (feed.LinkDb == null)
                        feed.LinkDb = link;
                }
                return feed;
            },
            param: new
            {
                HashId = hashId,
                IsAccessPrivate = false,
                Hide = req.Hides,
                PostStatus = StatusUtils.PostStatusIntPublic,
            }, splitOn: "Id, Id, Id, Id, Id");

        //Add view
        if (dbFeed == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        // Will map later
        var sound = await _soundService.GetSoundByPostAsync(dbFeed.Id);

        if ((dbFeed.Status == PostStatus.Draft && dbFeed.UserId != userId))
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        dbFeed.IsFollowing = await _context.Available<UserFollow>().AnyAsync(p => p.UserFollowerId == userId && p.UserFollowingId == dbFeed.UserId);
        dbFeed.IsFavorite = await _context.Available<SocialPostFavorite>().AnyAsync(p => p.UserId == userId && p.PostId == dbFeed.Id);

        dbFeed.Body = await _businessText.Process(dbFeed.Body);
        var result = MappingFeedRespone(dbFeed, sound);
        if (dbFeed.SharePostId.HasValue)
        {
            var sharePostInput = new SharePostInput
            {
                Id = dbFeed.SharePostId.Value,
                Type = dbFeed.SharePostType ?? SharePostType.Feed
            };
            var sharePost = await GetSharePosts(req, [sharePostInput]);
            result.SharePost = sharePost.Count > 0 ? sharePost.First() : null;
        }

        var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
        var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""), new
        {
            TargetIds = new List<Guid>() { result.Id },
            UserId = userId
        });

        if (postReactionResponse.Count() > 0)
        {
            MapReactionFeedDtoResponse(result, postReactionResponse.ToList());
        }

        return result;
    }

    public SharePostResponse MappingSharePostResponse(ShareBoxQueryResponse res)
    {
        if (res.Type != SharePostType.Feed)
        {
            return new SharePostResponse
            {
                Title = res.Title,
                Id = res.Id,
                HashId = res.HashId,
                Body = res.Body,
                ThumbnailUrl = res.ThumbnailUrl,
                Type = res.Type,
                Order = res.Order,
                IsMature = res.IsMature,
                LatestCreatedOn = res.LatestCreatedOn,
            };
        }
        var itemResponse = new SharePostResponse()
        {
            ThumbnailUrl = res.ThumbnailUrl,
            Body = HttpUtility.HtmlDecode(res.Body),
            CreatedOn = res.CreatedOn,
            HashId = res.HashId,
            Id = res.Id,
            ProfileId = res.ProfileId,
            UserId = res.UserId,
            MetaData = res.MetaDatas != null ? JsonConvert.DeserializeObject<MetaDataDto>(res.MetaDatas) : null,
            TotalResources = res.TotalResources,
            UserAvatar = res.UserAvatar,
            FullName = res.FullName,
            UserName = res.UserName,
            Resources = res.TotalResources > 0 && res.Resources != null ? JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources.ToString()) : new List<ResourceDto>(),
            Hide = res.Hide,
            Status = res.Status,
            Type = res.Type
        };
        itemResponse.MetaData.Description = HttpUtility.HtmlDecode(itemResponse.MetaData.Description);
        var link = res.Link != null ? JsonConvert.DeserializeObject<PostLinkFeedBoxResponse>(res.Link) : null;
        if (res.TotalResources > 0 && !string.IsNullOrEmpty(res.Resources))
        {
            itemResponse.Resources = new List<ResourceDto>();
            var resourceResponses = JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources);

            // Ensure not null
            resourceResponses = resourceResponses?.Where(p => p != null).OrderBy(p => p.Order).ToList();
            if (resourceResponses == null)
            {
                resourceResponses = [];
            }

            foreach (var i in resourceResponses)
            {
                if (i == null)
                {
                    continue;
                }

                i.Url = _sc.GetCdnUrl(i.Url, i.BucketName, i.MinioInstance, i.Type);

                itemResponse.Resources.Add(i);
            }
        }
        else if (link is not null)
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = link.HashId,
                Url = link.Url,
                Type = link.Type.ToDisplay()
            };
            itemResponse.Resources = new List<ResourceDto>()
                {
                    new ResourceDto()
                    {
                        HashId = link.HashId,
                        Url = link.Url,
                        Type = link.Type.ToResourceType(),
                }
                     };
        }

        return itemResponse;
    }


    public FeedBoxResponse MappingFeedBoxResponse(FeedBoxQueryResponse res, List<Guid>? postId, Guid? currentUserId)
    {
        var itemResponse = new FeedBoxResponse()
        {
            ThumbnailUrl = res.ThumbnailUrl,
            Body = HttpUtility.HtmlDecode(res.Body),
            CreatedOn = res.CreatedOn,
            HashId = res.HashId,
            Id = res.Id,
            ProfileId = res.ProfileId,
            UserId = res.UserId,
            MetaData = res.MetaDatas != null ? JsonConvert.DeserializeObject<MetaDataDto>(res.MetaDatas) : null,
            TotalResources = res.TotalResources,
            UserAvatar = res.UserAvatar,
            FullName = res.FullName,
            UserName = res.UserName,
            Resources = res.TotalResources > 0 && res.Resources != null ? JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources.ToString()) : new List<ResourceDto>(),
            Type = res.Type,
            CustomNote = res.CustomNote.ForLexical(),
            IsFavorite = postId == null ? false : postId.Contains(res.Id),
            IsCurrentUserAuthor = res.UserId == currentUserId,
            Hide = res.Hide,
            Status = res.Status,
            SharePostId = res.SharePostId,
            SharePostType = res.SharePostType,
        };
        itemResponse.MetaData.Description = HttpUtility.HtmlDecode(itemResponse.MetaData.Description);
        var link = res.Link != null ? JsonConvert.DeserializeObject<PostLinkFeedBoxResponse>(res.Link) : null;
        if (res.TotalResources > 0 && !string.IsNullOrEmpty(res.Resources))
        {
            itemResponse.Resources = new List<ResourceDto>();
            var resourceResponses = JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources);

            // Ensure not null
            resourceResponses = resourceResponses?.Where(p => p != null).OrderBy(p => p.Order).ToList();
            if (resourceResponses == null)
            {
                resourceResponses = [];
            }

            foreach (var i in resourceResponses)
            {
                if (i == null)
                {
                    continue;
                }

                i.Url = _sc.GetCdnUrl(i.Url, i.BucketName, i.MinioInstance, i.Type);

                itemResponse.Resources.Add(i);
                itemResponse.SubPosts.Add(new SubUploadFileDto
                {
                    Files = new List<UploadFileDto>() {
                        new UploadFileDto() {
                            Order = i.Order,
                            SubPostHashId = i.SubPostHashId,
                            HashId = i.HashId,
                            Height = i.Height,
                            Width = i.Width,
                            Url = i.Url,
                            Type = i.Type,
                            Name = i.Name
                        }
                    }
                });
            }
        }
        else if (link is not null)
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = link.HashId,
                Url = link.Url,
                Type = link.Type.ToDisplay()
            };
            itemResponse.Resources = new List<ResourceDto>()
                {
                    new ResourceDto()
                    {
                        HashId = link.HashId,
                        Url = link.Url,
                        Type = link.Type.ToResourceType(),
                }
                     };
        }

        return itemResponse;
    }

    public async Task<List<FeedBoxResponse>> GetFeedsByIds(PaginatedR req)
    {
        var hashIds = req.HashIds;
        var userId = req.UserId;

        var param = new
        {
            HashIds = hashIds?.Split(',').ToList(),
            Hide = req.Hides,
            PostStatus = StatusUtils.PostStatusIntPublic
        };
        var result = await _postRepository.Connection.QueryAsync<FeedBoxQueryResponse>(GetFeedBoxQuery, param);

        if (result != null && result.Any())
        {
            var postIds = await _context.Available<SocialPostFavorite>()
                .Where(p => p.UserId == userId)
                .Select(p => p.PostId)
                .ToListAsync();

            var listFeedDetails = new List<FeedBoxResponse>();

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""), new
            {
                TargetIds = result.Select(p => p.Id).ToList(),
                UserId = userId
            });

            var sharePostIds = result.Where(p => p.SharePostId.HasValue)
                .Select(p => new SharePostInput
                {
                    Id = p.SharePostId!.Value,
                    Type = p.SharePostType ?? SharePostType.Feed
                })
                .ToList();
            var sharePosts = await GetSharePosts(req, sharePostIds);

            var body = "";
            foreach (var i in result)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var commentsResult = await GetMostCommentReaction(result.Select(p => p.HashId + "").ToList());
            var comments = await GetReactionAndMentionOfComment(commentsResult.Comments, userId);

            foreach (var res in result)
            {
                var feed = MappingFeedBoxResponse(res, postIds, userId);

                feed.Body = await _businessText.Process(feed.Body, profiles);
                feed.IsCensored = !req.IsAdministrator && req.UserName != feed.UserName && feed.Status == PostStatus.Inactive;
                feed.IsBlur = feed.Status == PostStatus.Inactive;

                var postReaction = postReactionResponse.Where(p => p.TargetId == feed.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionFeedBoxResponse(feed, postReaction);
                }

                var sharePost = sharePosts.FirstOrDefault(p => p.Id == feed.SharePostId);
                if (sharePost != null)
                {
                    feed.SharePost = sharePost;
                }

                var totalComments = commentsResult.TotalComment?.FirstOrDefault(p => p.PostHashId == feed.HashId)?.TotalCommentCount;
                feed.Comments = new CommentPagedResults<MostReactionCommentResponse>(totalComments ?? 0, 1, 2)
                {
                    Items = comments.Where(p => p.PostHashId == feed.HashId),
                    TotalComments = totalComments ?? 0
                };

                listFeedDetails.Add(feed);
            }

            return listFeedDetails;
        }
        else
        {
            return [];
        }
    }

    private void MapReactionFeedBoxResponse(FeedBoxResponse item, List<CommentReactionResponseQuery> reactions)
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

    private void MapReactionSharePostResponse(SharePostResponse item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type!.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    private void MapReactionFeedDtoResponse(FeedDto item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type!.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    public async Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string keyWord, FeedSearchKeywordR feedLoadReq)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(SocialPost.CreatedOn);
            }
            var query = string.Format(GetAllFeedByKeyword, _postRepository.TableName, feedLoadReq.OrderBy, keyWord);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        PageSize = feedLoadReq.PageSize,
                        Offet = offset
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();
            foreach (var item in items)
            {
                listItemResponse.Add(MappingFeedInListRespone(item, null, null));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public FeedDisplayConfig GetFeedDisplayConfig()
    {
        return _feedDisplayConfig;
    }

    public async Task<bool> DeleteFeedAsync(IdBaseR request)
    {
        return await _postService.Delete(request);
    }

    public FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item, List<Guid>? postIds, bool? isMySelf)
    {
        var itemResponse = new FeedDto()
        {
            Id = item.Id,
            Title = item.Title,
            HashId = item.HashId,
            UserId = item.UserId,
            UserName = item.UserName,
            FullName = item.ProfileName,
            ProfileId = item.ProfileId,
            ThumbnailUrl = item.ThumbnailUrl,
            CreatedOn = item.CreatedOn,
            TotalResource = item.TotalResource,
            Type = item.Type,
            Status = item.Status,
            UserAvatar = item.UserAvatar,
            Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : [],
            CustomNote = item.CustomNote.ForLexical(),
            IsFavorite = postIds == null ? false : postIds.Contains(item.Id),
            Hide = item.Hide,
            IsCurrentUserAuthor = isMySelf,
            SharePostId = item.SharePostId
        };
        itemResponse.Body = HttpUtility.HtmlDecode(item.Body);
        var subPostHashIds = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostStr);
        #region Mapping with db query list
        if (item.TotalResource > 0 && !string.IsNullOrEmpty(item.SubPostResourceStr))
        {
            itemResponse.Resources = new List<ResourceDto>();
            var resourceResponses = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostResourceStr);

            // Ensure not null
            resourceResponses = resourceResponses?.Where(p => p != null).OrderBy(p => p.Order).ToList();
            if (resourceResponses == null)
            {
                resourceResponses = [];
            }

            foreach (var i in resourceResponses)
            {
                if (i == null)
                {
                    continue;
                }

                i.Url = _sc.GetCdnUrl(i.Url, i.BucketName, i.MinioInstance, i.Type);
                if (subPostHashIds.Any())
                {
                    i.SubPostHashId = subPostHashIds.FirstOrDefault(p => p.Id == i.SubPostId)?.HashId;
                }
                itemResponse.Resources.Add(i);
            }
        }
        else if (!string.IsNullOrEmpty(item.LinkUrl))
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = item.LinkHashId ?? "",
                Url = item.LinkUrl ?? "",
                Type = item.LinkType.ToDisplay()
            };
            itemResponse.Resources = new List<ResourceDto>()
                {
                    new ResourceDto()
                    {
                        HashId = item.LinkHashId,
                        Url = item.LinkUrl ?? "",
                        Type = item.LinkType.ToResourceType()
                    }
                };
        }
        if (item.MetaTitle != null && item.MetaDomain != null)
        {
            itemResponse.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDescription) ? HttpUtility.HtmlDecode(item.MetaDescription) : "",
                Domain = item.MetaDomain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaTitle) ? HttpUtility.HtmlDecode(item.MetaTitle) : "",
                Url = item.MetaUrl ?? ""
            };
        }

        #endregion

        return itemResponse;
    }

    private FeedDto MappingFeedRespone(FeedQueryDbDto item, BackgroundMedia.SearchDto? sound)
    {
        if (item == null)
            return new FeedDto();

        var itemResponse = new FeedDto()
        {
            Id = item.Id,
            Title = item.Title,
            HashId = item.HashId,
            UserId = item.UserId,
            FullName = item.ProfileName,
            UserName = item.UserName,
            ThumbnailUrl = item.ThumbnailUrl,
            CreatedOn = item.CreatedOn,
            ProfileId = item.ProfileId,
            Type = item.Type,
            Status = item.Status,
            UserAvatar = item.UserAvatar,
            Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
            Body = HttpUtility.HtmlDecode(item.Body),
            CustomNote = item.CustomNote.ForLexical(),
            IsFollowing = item.IsFollowing,
            Hide = item.Hide,
            IsFavorite = item.IsFavorite,
            SharePostId = item.SharePostId
        };

        #region Mapping with db query single
        bool hasResources = false;
        if (item.SubPostDbs != null && item.SubPostDbs.Count > 0)
        {
            itemResponse.SubPosts = new List<SubUploadFileDto>();
            foreach (var subPostdb in item.SubPostDbs)
            {
                if (subPostdb == null || subPostdb.FileDbs == null)
                {
                    continue;
                }

                var fileDbs = subPostdb.FileDbs.FirstOrDefault();
                if (fileDbs == null)
                {
                    continue;
                }

                var url = _sc.GetCdnUrl(fileDbs.Url, fileDbs.BucketName, fileDbs.MinioInstance, fileDbs.Type);

                itemResponse.Resources.Add(new ResourceDto
                {
                    HashId = fileDbs.HashId,
                    Type = fileDbs.Type,
                    Width = fileDbs.Width,
                    Height = fileDbs.Height,
                    Url = url,
                    Order = fileDbs.Order,
                    SubPostHashId = subPostdb.HashId,

                });
                var subPostResponse = new SubUploadFileDto()
                {
                    Id = subPostdb.Id,
                    HashId = subPostdb?.HashId,
                    Title = subPostdb.Title,
                    Name = subPostdb.Name,
                    ThumbnailUrl = subPostdb.ThumbnailUrl,
                    Permission = subPostdb.Permission,
                    CreatedOn = subPostdb.CreatedOn,
                    PublishDate = subPostdb.PublishDate,
                    Status = subPostdb.Status,
                    Body = subPostdb.Body
                };
                if (subPostdb.FileDbs != null)
                {
                    hasResources = true;

                    subPostResponse.Files = subPostdb.FileDbs.Select(x =>
                    {
                        var resource = new UploadFileDto
                        {
                            HashId = subPostdb.HashId,
                            Url = _sc.GetCdnUrl(x.Url, x.BucketName, x.MinioInstance, x.Type),
                            Name = x.Name,
                            Type = x.Type,
                            Status = x.Status,
                            Width = x.Width,
                            Height = x.Height,
                            Order = x.Order
                        };

                        return resource;
                    }).ToList();
                }
                itemResponse.SubPosts.Add(subPostResponse);
            }
        }
        if (item.MetaDataDb != null)
        {
            itemResponse.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDataDb.Description) ? HttpUtility.HtmlDecode(item.MetaDataDb.Description) : "",
                Domain = item.MetaDataDb.Domain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaDataDb.Title) ? HttpUtility.HtmlDecode(item.MetaDataDb.Title) : "",
                Url = item.MetaDataDb.Url ?? ""
            };
        }
        if (item.LinkDb != null && !string.IsNullOrEmpty(item.LinkDb.HashId) && !hasResources)
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = item.LinkDb.HashId ?? "",
                Url = item.LinkDb.Url ?? "",
                Type = item.LinkDb.Type.ToDisplay()
            };
        }

        // Will map later
        itemResponse.BackgroundSound = sound;
        #endregion
        itemResponse.TotalResource = itemResponse.Resources.Count;
        return itemResponse;
    }
    private static string AddAdditionalFeedQuery(FeedLoadReq feedLoadReq, string query, LoadFeedType loadFeedType)
    {
        var additionalTotalQuery = @"INNER JOIN identity.""Users"" u ON u.""Id"" = p.""UserId"" ";
        var additionalTotalCondition = @$"AND u.""UserName"" = '{feedLoadReq.NewUserName}'";
        var additionalCondition = @$"AND u.""UserName"" = '{feedLoadReq.NewUserName}'";

        query = query.Replace("[AdditionalCondition]", additionalCondition)
            .Replace("[AdditionalTotalQuery]", additionalTotalQuery)
            .Replace("[AdditionalTotalCondition]", additionalTotalCondition);

        return query;
    }

    private void MapPostBoxReactionResponse(FeedDto item, List<CommentReactionResponseQuery> reactions)
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

    private async Task<TrackingSummaryGetFeedsRsp> GetFeeds(int pageNumber, int pageSize, string frDate, string toDate, Guid? userId, string postHideIds)
    {
        var res = new TrackingSummaryGetFeedsRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new TrackingSummaryProto.TrackingSummaryProtoClient(channel);

            var request = new TrackingSummaryGetFeedsReq
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                FrDate = frDate,
                ToDate = toDate,
                UserId = userId?.ToString(),
                PostHideIds = postHideIds
            };
            var rsp = await client.GetFeedsAsync(request);

            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
            res.TotalRecords = rsp.TotalRecords;
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
            Console.WriteLine($"[FEED GRPC FAIL] Analytic gRPC error: {ex.Message}");
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<CommentsResult> GetMostCommentReaction(List<string> hashIds)
    {
        var res = new CommentsResult();

        var paramValues = new
        {
            HashIds = hashIds,
        };
        var schema = "social";
        var @params = "@HashIds";

        var fn = "social.fw_get_most_reaction_comments";
        res.Comments = await _postRepository.Connection.QueryAsync<MostReactionCommentResponse>(fn.ToFn(schema, schema, @params), paramValues);

        fn = "social.fw_get_total_comment_counts";
        res.TotalComment = await _postRepository.Connection.QueryAsync<CommentCount>(fn.ToFn(schema, schema, @params), paramValues);

        return res;
    }

    private async Task<IEnumerable<MostReactionCommentResponse>> GetReactionAndMentionOfComment(IEnumerable<MostReactionCommentResponse>? comments, Guid? userId)
    {
        if (comments == null)
        {
            return [];
        }

        var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialPostCommentReactions""");
        var postCommentReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
        {
            TargetIds = comments?.Where(p => p.Order == null).Select(p => p.Id).ToList(),
            UserId = userId
        });

        var querySubPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialSubPostCommentReactions""");
        var subPostCommentReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(querySubPostCommentReaction, new
        {
            TargetIds = comments?.Where(p => p.Order != null).Select(p => p.Id).ToList(),
            UserId = userId
        });

        foreach (var comment in comments)
        {
            var postCommentReaction = postCommentReactionResponse.Where(p => p.TargetId == comment.Id).ToList();
            if (postCommentReaction.Count > 0)
            {
                MapReactionResponse(comment, postCommentReaction);
            }

            var subPostCommentReaction = subPostCommentReactionResponse.Where(p => p.TargetId == comment.Id).ToList();
            if (subPostCommentReaction.Count > 0)
            {
                MapReactionResponse(comment, subPostCommentReaction);
            }

            comment.ResourceUrl = await _sc.GetPublicUrl(comment.ResourceUrl, comment.BucketName, comment.MinioInstance);
            comment.Body = await _businessText.Process(comment.Body);
        }

        return comments;
    }

    private void MapReactionResponse(MostReactionCommentResponse item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Where(p => p.Type != null).Select(x => new ReactionResponse { Count = x.Count, Type = x.Type.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    #endregion

    #region -- Classes --

    public class CommentCount
    {
        public string? PostHashId { get; set; }
        public int TotalCommentCount { get; set; }
    }

    public class CommentsResult
    {
        public IEnumerable<MostReactionCommentResponse>? Comments { get; set; }
        public IEnumerable<CommentCount>? TotalComment { get; set; }
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

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    /// <summary>
    /// Post service
    /// </summary>
    private readonly IPostService _postService;

    /// <summary>
    /// Sound service
    /// </summary>
    private readonly ISoundService _soundService;

    private readonly IRepository<SocialPost> _postRepository;
    private readonly IMapper _mapper;
    private readonly FeedDisplayConfig _feedDisplayConfig;

    #endregion
}
