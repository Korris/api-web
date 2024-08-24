using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class FeedService : IFeedService
{
    #region -- Methods --

    public FeedService(
        IMcsgContext context,
        ISetting setting,
        IStorageClient sc,
        IPostService postService,
        IMetaDataService metaDataService,
        ITagService tagService,
        IFileService fileService,
        ICurrentUserService currentUserService,
        ISoundService soundService,
        IPostLinkService postLinkService,
        ISmartLookupService smartLookupService,
        IBusinessText businessBodyText,
        IUnitOfWork unitOfWork,
        ISmartCountService smartCountService,
        IViewHistoryService viewHistoryService,
        IConfiguration configuration,
        IOptionsMonitor<FeedDisplayConfig> feedDisplayConfig,
        IMapper mapper)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _postService = postService;
        _metaDataService = metaDataService;
        _tagService = tagService;
        _fileService = fileService;
        _currentUserService = currentUserService;
        _soundService = soundService;
        _postLinkService = postLinkService;
        _smartLookupService = smartLookupService;
        _businessText = businessBodyText;

        _unitOfWork = unitOfWork;
        _postRepository = unitOfWork.GetRepository<SocialPost>();

        _smartCountService = smartCountService;
        _viewHistoryService = viewHistoryService;
        _configuration = configuration;
        _feedDisplayConfig = feedDisplayConfig.CurrentValue;
        _mapper = mapper;
    }

    public async Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);
            var date = DateTime.UtcNow.Date;

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(SocialPost.CreatedOn);
            }
            var query = "";
            if (loadFeedType == LoadFeedType.TRENDING || loadFeedType == LoadFeedType.HOT)
            {
                query = string.Format(GetAllFeedsWithTopCommentQuery, _postRepository.TableName, feedLoadReq.OrderBy);
                if (loadFeedType == LoadFeedType.HOT)
                {
                    int offsetDate = date.DayOfWeek - DayOfWeek.Monday;
                    DateTime lastMonday = date.AddDays(-offsetDate);
                    date = lastMonday;
                }
            }
            else
            {
                query = string.Format(GetAllFeedsQuery, _postRepository.TableName, feedLoadReq.OrderBy);
            }

            query = AddAdditionalFeedQuery(feedLoadReq, query, loadFeedType);

            var isMySelf = feedLoadReq.NewUserName == feedLoadReq.UserName;

            if (feedLoadReq.NewUserName != null)
            {
                var newUserNameQuery = $@"OR (u.""UserName"" = @NewUserName AND @MySelf)";
                query = query.Replace("[AddNewUserNameContidion]", newUserNameQuery);
            }
            else
            {
                query = query.Replace("[AddNewUserNameContidion]", "");
            }

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        Type = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        feedLoadReq.PageSize,
                        Offet = offset,
                        Date = date,
                        Status = PostStatus.Public,
                        DateOnly = DateOnly.FromDateTime(date),
                        Hide = feedLoadReq.Hides,
                        MySelf = isMySelf,
                        feedLoadReq.NewUserName,
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();

            var userId = feedLoadReq.UserId;
            var postIds = await _context.SocialPostFavoriteAvailable.Where(p => p.UserId == userId).Select(p => p.PostId).ToListAsync();

            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            foreach (var item in items)
            {
                item.Body = await _businessText.Process(item.Body, profiles);
                listItemResponse.Add(MappingFeedInListRespone(item, postIds));
            }

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
                var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""), new
                {
                    TargetIds = items.Select(p => p.Id).ToList(),
                    UserId = userId
                });

                if (postReactionResponse.Count() > 0)
                {
                    foreach (var item in listItemResponse)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Count > 0)
                        {
                            MapReactionFeedDtoResponse(item, postReaction);
                        }
                    }
                }
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
                                   AND qpost.""Status""=@PostStatus
                                   AND qpost.""IsDelete""=false";
        }
        else
        {
            queryCondition = $@" WHERE qpost.""Body"" ILIKE '%{feedLoadReq.Keyword}%'
                                     AND qpost.""Type""=@PostType
                                     AND qpost.""Status""=@PostStatus
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
                       PostStatus = (int)PostStatus.Public
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
            listItemResponse.Add(MappingFeedInListRespone(item, null));
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
                listItemResponse.Add(MappingFeedInListRespone(item, null));
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

    public async Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId, Guid userId)
    {
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
                             'Height', r.""Height"",
                             'Width', r.""Width"",
                             'BucketName', r.""BucketName"",
                             'Type', r.""Type"",
                             'Name', r.""Name"",
                             'Order',r.""Order"",
                             'HashId',r.""HashId"",
                             'SubPostHashId',sp.""HashId""
                
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
                         sp.""HashId"" as SubPostHashId,
                         sp.""Id"",
                         sp.""CreatedOn"", 
                         sp.""Body"",
                         sp.""CreatedBy"",
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
        var resource = JsonConvert.DeserializeObject<List<ResourceDto>>(dataQuery.ResourcesStr);
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
            if (item.Type == ResourceType.Video || item.Type == ResourceType.Audio)
            {
                item.Url = await _sc.Strategy.PresignedGetObject(item.Url, _setting.Minio.MaxExpiryInSeconds, null);
            }
            else
            {
                item.Url = _setting.Api.Web.Media.GetMediaPath(item.Name, item.Url);
            }
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
             }
        });

        var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
        var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialSubPostReactions"""), new
        {
            TargetIds = new List<Guid>() { data.Id },
            UserId = userId
        });

        if (postReactionResponse.Count() > 0)
        {
            MapReactionFeedDtoResponse(data, postReactionResponse.ToList());
        }

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
                Hide = req.Hides
            }, splitOn: "Id, Id, Id, Id, Id");

        //Add view
        if (dbFeed == null)
        {
            throw new NotFoundException(E204, M204);
        }

        // Will map later
        var sound = await _soundService.GetSoundByPostAsync(dbFeed.Id);

        //await _viewHistoryService.QueueAddView(userId, dbFeed.Id, EntityType.Post, "", EntitySubType.Sub1);

        if (dbFeed.Status == PostStatus.Inactive || (dbFeed.Status == PostStatus.Draft && dbFeed.UserId != userId))
        {
            throw new NotFoundException(E204, M204);
        }

        dbFeed.IsFollowing = await _context.UserFollowAvailable.AnyAsync(p => p.UserFollowerId == userId && p.UserFollowingId == dbFeed.UserId);

        dbFeed.Body = await _businessText.Process(dbFeed.Body);

        var result = MappingFeedRespone(dbFeed, sound);
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
            Hide = res.Hide
        };
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

            foreach (var resourceResponse in resourceResponses)
            {
                if (resourceResponse == null)
                {
                    continue;
                }

                if (resourceResponse.Type == ResourceType.Video || resourceResponse.Type == ResourceType.Audio)
                {
                    resourceResponse.Url = _sc.Strategy.PresignedGetObject(resourceResponse.Url, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                }
                else
                {
                    resourceResponse.Url = _setting.Api.Web.Media.GetMediaPath(resourceResponse.Name, resourceResponse.Url);
                }

                itemResponse.Resources.Add(resourceResponse);
                itemResponse.SubPosts.Add(new SubUploadFileDto
                {
                    Files = new List<UploadFileDto>() {
                        new UploadFileDto() {
                            Order = resourceResponse.Order,
                            SubPostHashId = resourceResponse.SubPostHashId,
                            HashId = resourceResponse.HashId,
                            Height = resourceResponse.Height,
                            Width = resourceResponse.Width,
                            Url = resourceResponse.Url,
                            Type = resourceResponse.Type,
                            Name = resourceResponse.Name
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

    public async Task<List<FeedBoxResponse>> GetFeedsByIds(FeedHashIdsR req)
    {
        var hashIds = req.HashIds;
        var userId = req.UserId;

        var param = new { HashIds = hashIds.Split(',').ToList(), Hide = req.Hides };
        var result = await _postRepository.Connection.QueryAsync<FeedBoxQueryResponse>(GetFeedBoxQuery, param);
        var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;

        var postIds = await _context.SocialPostFavoriteAvailable.Where(p => p.UserId == userId)
                                                                .Select(p => p.PostId)
                                                                .ToListAsync();
        if (result != null && result.Any())
        {
            var listFeedDetails = new List<FeedBoxResponse>();

            foreach (var res in result)
            {
                res.Body = await _businessText.Process(res.Body);
                listFeedDetails.Add(MappingFeedBoxResponse(res, postIds, currentUserId));
            }

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"social.""SocialPostReactions"""), new
            {
                TargetIds = listFeedDetails.Select(p => p.Id).ToList(),
                UserId = userId
            });

            if (postReactionResponse.Count() > 0)
            {
                foreach (var item in listFeedDetails)
                {
                    var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                    if (postReaction.Count > 0)
                    {
                        MapReactionFeedBoxResponse(item, postReaction);
                    }
                }
            }

            return listFeedDetails;
        }
        else
        {
            return new List<FeedBoxResponse>(); // Trả về danh sách rỗng nếu không có kết quả
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

    private void MapReactionFeedDtoResponse(FeedDto item, List<CommentReactionResponseQuery> reactions)
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
                listItemResponse.Add(MappingFeedInListRespone(item, null));
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

    public async Task<bool> DeleteFeedAsync(Guid postId)
    {
        return await _postService.Delete(postId);
    }

    public FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item, List<Guid>? postIds)
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
            Hide = item.Hide
        };
        itemResponse.Body = HttpUtility.HtmlDecode(item.Body);
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

            foreach (var resourceResponse in resourceResponses)
            {
                if (resourceResponse == null)
                {
                    continue;
                }

                if (resourceResponse.Type == ResourceType.Video || resourceResponse.Type == ResourceType.Audio)
                {
                    resourceResponse.Url = _sc.Strategy.PresignedGetObject(resourceResponse.Url, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                }
                else
                {
                    resourceResponse.Url = _setting.Api.Web.Media.GetMediaPath(resourceResponse.Name, resourceResponse.Url);
                }
                itemResponse.Resources.Add(resourceResponse);
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
            Hide = item.Hide
        };

        #region Mapping with db query single
        bool hasResources = false;
        if (item.SubPostDbs != null && item.SubPostDbs.Count > 0)
        {
            itemResponse.SubPosts = new List<SubUploadFileDto>();
            foreach (var subPostdb in item.SubPostDbs)
            {
                var fileDbs = subPostdb.FileDbs.FirstOrDefault();
                var url = "";
                if (fileDbs.Type == ResourceType.Video || fileDbs.Type == ResourceType.Audio)
                {
                    url = _sc.Strategy.PresignedGetObject(fileDbs.Url, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                }
                else
                {
                    url = _setting.Api.Web.Media.GetMediaPath(fileDbs.Name, fileDbs.Url);
                }
                itemResponse.Resources.Add(new ResourceDto
                {
                    HashId = subPostdb.HashId,
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
                            Url = _setting.Api.Web.Media.GetMediaPath(x.Name, x.Url),
                            Name = x.Name,
                            Type = x.Type,
                            Status = x.Status,
                            Width = x.Width,
                            Height = x.Height,
                            Order = x.Order
                        };
                        if (x.Type == ResourceType.Audio || x.Type == ResourceType.Video)
                        {
                            resource.Url = _sc.Strategy.PresignedGetObject(x.Url, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                        }
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
        if (string.IsNullOrEmpty(feedLoadReq.NewUserName))
        {
            query = query.Replace("[AdditionalCondition]", "")
                .Replace("[AdditionalTotalQuery]", "")
                .Replace("[AdditionalTotalCondition]", "");
        }
        else
        {
            var additionalTotalQuery = @"INNER JOIN identity.""Users"" u ON u.""Id"" = p.""UserId"" ";
            var additionalTotalCondition = @$"AND u.""UserName"" = '{feedLoadReq.NewUserName}'";
            var additionalCondition = @$"AND u.""UserName"" = '{feedLoadReq.NewUserName}'";

            query = query.Replace("[AdditionalCondition]", additionalCondition)
                .Replace("[AdditionalTotalQuery]", additionalTotalQuery)
                .Replace("[AdditionalTotalCondition]", additionalTotalCondition);
        }
        return query;
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
    /// Post service
    /// </summary>
    private readonly IPostService _postService;

    /// <summary>
    /// MetaData service
    /// </summary>
    private readonly IMetaDataService _metaDataService;

    /// <summary>
    /// Tag service
    /// </summary>
    private readonly ITagService _tagService;

    /// <summary>
    /// File service
    /// </summary>
    private readonly IFileService _fileService;

    /// <summary>
    /// CurrentUser service
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    /// Sound service
    /// </summary>
    private readonly ISoundService _soundService;

    /// <summary>
    /// PostLink service
    /// </summary>
    private readonly IPostLinkService _postLinkService;

    /// <summary>
    /// SmartLookup service
    /// </summary>
    private readonly ISmartLookupService _smartLookupService;

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    private readonly IRepository<SocialPost> _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISmartCountService _smartCountService;
    private readonly IViewHistoryService _viewHistoryService;
    private readonly IConfiguration _configuration;
    private readonly FeedDisplayConfig _feedDisplayConfig;
    private readonly IMapper _mapper;

    #endregion
}
