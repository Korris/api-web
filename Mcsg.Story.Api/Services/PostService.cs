using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Constants;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Interfaces;
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
    private readonly IRepository<StoryPost> _postRepository;
    private readonly IRepository<StoryPostComment> _postCommentRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IRepository<StorySubPost> _subPostRepository;
    private readonly IRepository<StoryPostReport> _postReportRepository;
    private readonly IValidator<StoryPostReport> _postReportValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITagService _tagService;
    private readonly IUserService _userService;
    private readonly IFileService _fileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISmartLookupService _smartLookupService;
    private readonly IViewHistoryService _viewHistoryService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PostService(IUnitOfWork unitOfWork,
        ITagService tagService,
        IRepository<SmartLookup> smartLookupRepository,
        IUserService userService,
        IFileService fileService,
        ICurrentUserService currentUserService,
        IViewHistoryService viewHistoryService,
        IConfiguration configuration,
        IMapper mapper,
        IMcsgContext context,
        ISetting setting,
        ISmartLookupService smartLookupService,
        IValidator<StoryPostReport> postReportValidator,
        IRepository<StoryPostComment> postCommentRepository)
    {
        _postRepository = unitOfWork.GetRepository<StoryPost>();
        _subPostRepository = unitOfWork.GetRepository<StorySubPost>();
        _postReportRepository = unitOfWork.GetRepository<StoryPostReport>();
        _unitOfWork = unitOfWork;
        _tagService = tagService;
        _userService = userService;
        _fileService = fileService;
        _currentUserService = currentUserService;
        _viewHistoryService = viewHistoryService;
        _smartLookupService = smartLookupService;
        _configuration = configuration;
        _mapper = mapper;
        _context = context;
        _setting = setting;
        _postReportValidator = postReportValidator;
        _smartLookupRepository = smartLookupRepository;
        _postCommentRepository = postCommentRepository;
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
    public async Task<PostSeriesResponse> PostSeries(PostType type, ComicPostSeriesR comicPostReq)
    {
        var ss = _currentUserService.Session;
        var currentUserId = ss.UserId;
        var profileId = ss.ProfileId;
        var currentFullName = ss.ProfileName;

        VerifyBasicInfo(comicPostReq.Title);

        //Check first post
        var rewards = await CheckRewardsForPost(currentUserId, type);

        var hashId = PostConfig.HashLength.GetRandomString();
        //var safePlainString = "";
        //if (!string.IsNullOrEmpty(comicPostReq.Summary))
        //{
        //    safePlainString = System.Web.HttpUtility.HtmlEncode(comicPostReq.Summary);
        //}

        var post = new StoryPost()
        {
            Title = comicPostReq.Title,
            Type = type,
            HashId = hashId,
            UserId = currentUserId,
            AuthorId = comicPostReq.IsCurrentUserIsAuthor ? currentUserId : null,
            AuthorName = comicPostReq.IsCurrentUserIsAuthor ? currentFullName : comicPostReq.AuthorName,
            Body = comicPostReq.Summary,
            ThumbnailUrl = comicPostReq.ThumbnailUrl,
            CoverUrl = comicPostReq.CoverUrl,
            IsMature = comicPostReq.IsMature,
            Permission = comicPostReq.Permission,
            Status = PostStatus.Public,
            CreatedBy = currentUserId,
            //TODO FAKE DATA
            ViewCount = 0

        };
        var result = new NewPostSeriesResponse
        {
            Id = post.Id,
            Title = comicPostReq.Title,
            HashId = hashId,
            UserId = currentUserId,
            Type = post.Type,
            ThumbnailUrl = post.ThumbnailUrl,
            CreatedOn = post.CreatedOn,
            Status = post.Status,
            Body = comicPostReq.Summary,
            CoverUrl = comicPostReq.CoverUrl,
            IsMature = comicPostReq.IsMature,
            Permission = comicPostReq.Permission,
            ProfileId = profileId,
            AuthorName = post.AuthorName,
            IsCurrentUserIsAuthor = comicPostReq.IsCurrentUserIsAuthor,
            Rewards = rewards
        };
        try
        {
            await _postRepository.InsertAsync(post);

            await _smartLookupRepository.InsertAsync(new SmartLookup
            {
                CountCriteria = 0,
                Keyword = comicPostReq.Title,
                KeywordType = LookupKeywordType.Story
            });

            if (comicPostReq.Tags != null && comicPostReq.Tags.Count > 0)
            {
                result.Tags = (await _tagService.AddTagsToPost(post.Id, comicPostReq.Tags, currentUserId)).ToArray();
            }
        }
        catch (Exception)
        {
            _unitOfWork.RollbackTransaction();
            throw;
        }

        return result;
    }

    public async Task<PostSeriesResponse> GetSeries(string hashId, bool isLoadChapters)
    {
        var query = string.Format(GetSeriesQuery, _postRepository.TableName);
        string subNotLoadChapter = (isLoadChapters ? "" : @" AND sp.""Id"" IS NULL ");
        query = query.Replace("[Not-load-chapter]", subNotLoadChapter);

        var currentUserId = _currentUserService?.Session?.UserId;

        //TODO Premium            
        query = AddWithPermission(query, currentUserId);
        //
        PostSeriesQueryDbResponse dbPost = null;
        await _postRepository
            .Connection.QueryAsync<PostSeriesQueryDbResponse, ChapterBasicResponse, PostSeriesQueryDbResponse>(query,
            (feed, subpost) =>
            {
                if (dbPost == null)
                {
                    dbPost = feed;
                    if (dbPost.Tags != null && dbPost.Tags.Length > 1)
                        dbPost.Tags = dbPost.Tags.Distinct().ToArray();
                }
                if (subpost != null)
                {
                    if (dbPost.Chapters == null)
                        dbPost.Chapters = new List<ChapterBasicResponse>();
                    //Mapping schedule
                    subpost.IsPublicNow = CheckIsPublicNow(subpost.PublishDate);
                    if (subpost != null && subpost.Id != Guid.Empty)
                    {
                        subpost.ViewCount = subpost.ViewCount ?? 0;
                        dbPost.Chapters.Add(subpost);
                    }

                }
                return feed;
            },
            param: new
            {
                HashId = hashId,
                IsAccessPrivate = false,
                CurrentDate = DateTime.UtcNow,
                UserId = currentUserId
            }, splitOn: "Id, Id");
        //Add view
        if (dbPost == null)
        {
            throw new NotFoundException(E204, M204);
        }
        if (currentUserId != null)
        {
            //await _viewHistoryService.QueueAddView(currentUserId ?? Guid.Empty, dbPost.Id, EntityType.POST, "", (EntitySubType)(dbPost.Type));
        }
        if (dbPost.Status == PostStatus.Inactive || (dbPost.Status == PostStatus.Draft && dbPost.UserId != currentUserId))
        {
            throw new NotFoundException(E204, M204);
        }
        dbPost.TotalComment = await _postRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = hashId });
        dbPost.IsFollowing = currentUserId == null ? false : await _context.StoryPostFavorites.AnyAsync(p => p.CreatedBy == currentUserId && p.PostId == dbPost.Id);
        return MappingFeedRespone(dbPost);
    }
    public async Task<ChapterResponse> GetSeriesChapter(string hashId, float order)
    {
        var query = string.Format(GetSeriesChapterByHashIdWithJoinOrder, _postRepository.TableName);
        var userIsPremium = _currentUserService?.Session?.IsPremium ?? false;
        var currentUserId = _currentUserService?.Session?.UserId;

        ChapterResponse subpost = null;
        await _subPostRepository
            .Connection.QueryAsync<ChapterResponse, StoryResource, ChapterResponse>(query,
            (subpostdb, resource) =>
            {
                if (subpostdb == null)
                {
                    subpost = subpostdb;
                    subpost.Body = System.Web.HttpUtility.HtmlDecode(subpostdb.Body);
                    subpost.IsPublicNow = CheckIsPublicNow(subpostdb.PublishDate);
                }
                if (subpostdb != null)
                {
                    if (subpost?.Id != subpostdb.Id)
                    {
                        subpost = subpostdb;
                        subpost.Body = System.Web.HttpUtility.HtmlDecode(subpostdb.Body);
                        subpost.IsPublicNow = CheckIsPublicNow(subpostdb.PublishDate);
                        if (subpost.IsPublicNow)
                        {
                            subpost.PublishDate = null;
                        }
                    }
                    if (subpost != null && subpost.Files == null)
                        subpost.Files = new List<UploadFileDto>();
                    if (resource != null)
                    {
                        subpost.Files.Add(MappingFile(resource));
                    }

                }
                return subpostdb;
            },
            param: new
            {
                PostHashId = hashId,
                IsAccessPrivate = false,
                SubPostOrder = order,
                UserId = currentUserId
            }, splitOn: "Id, Id");


        if (subpost != null)
        {
            if (subpost.CreatedBy != currentUserId && subpost.UserId != currentUserId)
            {
                if (subpost.PublishDate != null && subpost.PublishDate < DateTime.UtcNow)
                {
                    throw new BadRequestException(E204, M204);
                }
                //Check permission
                if (subpost.Permission == PostPermission.Private)
                {
                    throw new BadRequestException(E204, M204);
                }
                //Check IsExclusive
                if (subpost.IsExclusive && subpost.UserExclusiveId == null)
                {
                    throw new BadRequestException(ApiErrorCode.NEED_BUY_TO_READ, ApiErrorMessage.NEED_BUY_TO_READ);
                }
                if (subpost.Permission == PostPermission.Premium && (!userIsPremium && subpost.UserExclusiveId == null))
                {
                    //Todo implement Premium
                    throw new BadRequestException(ApiErrorCode.NEED_PREMIUM_TO_READ, ApiErrorMessage.NEED_PREMIUM_TO_READ);
                }
            }


            //Add view
            if (currentUserId != null)
            {
                //await _viewHistoryService.QueueAddView(currentUserId ?? Guid.Empty, subpost.Id, EntityType.SUBPOST, "", null);
            }
        }
        else
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, ApiErrorMessage.CHAPTER_NOT_EXIST);
        }


        return subpost;
    }
    public async Task<PostSeriesAllTopResponse> GetTopSeries(PostType type)
    {
        var result = new PostSeriesAllTopResponse();
        string allSubQuery = $@"({GetTopPostHitQuery})
                        UNION ALL
                        ({GetTopLatestPostHitQuery})
                        UNION ALL
                        ({GetTopLatestCompletePostHitQuery})";

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", allSubQuery)
            .Replace("[CountResults]", "")
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
            .Replace("[OrderBy]", "CreatedOn");

        var dbFeed = await _postRepository
            .Connection.QueryAsync<PostSeriesTopQueryDbResponse>(query,
            new
            {
                PostType = type,
                PageSize = 10,
                Offet = 0,
                LastWeek = (DateTime.UtcNow.AddDays(-7)),
                PostStatus = (int)PostStatus.Public
            });
        result = new PostSeriesAllTopResponse();// MappingTopSeries(dbFeed);
        var listHit = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.HIT).ToList();
        var listLatest = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.LATEST).ToList();
        var listLatestCompleted = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.COMPLETED).ToList();
        result.TopHits = MappingTopSeries(listHit);
        result.TopLatest = MappingTopSeries(listLatest);
        result.TopCompleted = MappingTopSeries(listLatestCompleted);

        return result;
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, ComicPostListSeriesR request)
    {
        var currentUserId = _currentUserService?.Session?.UserId;
        var isFavorite = currentUserId == null ? false : request.IsFavorite;

        var offset = request.PageSize * (request.PageNumber - 1);

        string allSubQuery = $@"
                        ({GetTopLatestPostByTagQuery})";
        string countTopQuery = PaginationCountResult;

        if (isFavorite && request.HashTag == null)
        {
            allSubQuery = $@"
                        ({GetTopLatestPostByFavoriteQuery})";
            countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestPostByFavoriteToCountQuery);
        }
        else
        {
            countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestPostByTagToCountQuery);
        }

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", allSubQuery)
            .Replace("[CountResults]", countTopQuery)
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
            .Replace("[OrderBy]", "CreatedOn");

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = type,
                    PageSize = request.PageSize,
                    Offet = offset,
                    LastWeek = (DateTime.UtcNow.AddDays(-7)),
                    PostStatus = (int)PostStatus.Public,
                    PostPermission = (int)PostPermission.Public,
                    TagName = request.HashTag,
                    UserId = currentUserId
                });

        var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        var items = MapTopSeries(dbFeed.ToList());
        if (items != null && items.Count() > 0)
        {
            var results = new PagedResponse<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
            results.Items = items;
            return results;
        }
        else
        {
            return new PagedResponse<PostSeriesTopResponse>(0);
        }
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, ComicRelationPostSeriesR request)
    {
        try
        {
            #region Get post
            var queryGetPost = string.Format(GetPostWithHashId, _postRepository.TableName);
            var post = await _postRepository.Connection.QueryFirstAsync<StoryPost>(queryGetPost, new { HashId = request.HashId });
            #endregion

            if (post == null)
            {
                throw new NotFoundException(E204, M204);
            }

            var offset = request.PageSize * (request.PageNumber - 1);

            string whereClause = " WHERE qpost1.\"Type\" = @PostType AND qpost1.\"Status\" = @PostStatus AND qpost1.\"IsDelete\" = false AND qpost1.\"HashId\" != @HashId ";
            var tags = await _tagService.GetTagsByPostIdAsync(post.Id);
            var tagIds = new List<Guid>();
            if (tags != null && tags.Any())
            {
                tagIds = tags.Select(x => x.Id).ToList();
                whereClause += " AND ( qpost1.\"CreatedBy\" = @AuthorId OR qtag.\"Id\" = ANY(@TagIds) ) ";
            }
            else
            {
                whereClause += " AND qpost1.\"CreatedBy\" = @AuthorId ";
            }

            string allSubQuery = $@"({GetTopLatestPostByMultiTagQuery})";
            string countTopQuery = PaginationCountResult;

            countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetTopLatestPostByMultiTagToCountQuery.Replace("[WhereMainQuery]", whereClause));

            var query = GetRelatedPostQuery.Replace("[SelectPostIdsQuery]", allSubQuery)
                .Replace("[WhereMainQuery]", whereClause)
                .Replace("[CountResults]", countTopQuery)
                .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
                .Replace("[OrderBy]", "CreatedOn");

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = type,
                        PageSize = request.PageSize,
                        Offet = offset,
                        LastWeek = (DateTime.UtcNow.AddDays(-7)),
                        PostStatus = (int)PostStatus.Public,
                        TagIds = tagIds,
                        AuthorId = post.CreatedBy.Value,
                        HashId = request.HashId
                    });

            var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var items = MapTopSeries(dbFeed.ToList());
            if (items != null && items.Count() > 0)
            {
                var results = new PagedResponse<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
                results.Items = items;
                return results;
            }
            else
            {
                return new PagedResponse<PostSeriesTopResponse>(0);
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, ComicTopPostR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(selectedType);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    LastWeek = (DateTime.UtcNow.AddDays(-7)),
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    PostStatus = (int)PostStatus.Public
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

    public async Task UpdateKeyWordForComicAndStoryToSmartLookup()
    {
        var queryNameListPost = $@"SELECT ""Title"" FROM ""story"".""StoryPosts""  where ""Type"" != {(int)PostType.Feed}  AND ""IsDelete"" = false ";
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
    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, ComicTopPostR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_USER);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    PostStatus = (int)PostStatus.Public,
                    ProfileName = profileName
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

    public async Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, ComicPostByTagNameR input)
    {
        ValidateTotalItem(input.PageSize);
        PagedResponse<PostBoxResposne> results;
        var offset = input.PageSize * (input.PageNumber - 1);

        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_TAG);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = input.PageSize,
                    Offet = offset,
                    PostStatus = (int)PostStatus.Public,
                    TagName = input.TagName
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostBoxResposne>(totalItems, input.PageNumber, input.PageSize);
            results.Items = MappingToPostBoxResponse(items);
        }
        else
        {
            results = new PagedResponse<PostBoxResposne>(0);
        }
        return results;
    }

    public async Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, ComicPostByProFileNameR input)
    {
        ValidateTotalItem(input.PageSize);
        PagedResponse<PostBoxResposne> results;
        var offset = input.PageSize * (input.PageNumber - 1);

        var queryCondition = "";
        if (input.SearchBy == "ProfileName")
        {
            queryCondition = $@"WHERE u.""ProfileName""=@ProfileName 
                                   AND p.""Type""=@PostType
                                   AND p.""Status""=@PostStatus                                   
                                   AND p.""Permission""=@Permission
                                   AND p.""IsDelete""=false";
        }
        else
        {
            queryCondition = $@" WHERE p.""Title"" ILIKE '%{input.Keyword}%'
                                     AND p.""Type""=@PostType
                                     AND p.""Status""=@PostStatus
                                     AND p.""Permission""=@Permission
                                     AND p.""IsDelete""=false";
        }

        var query = $@"SELECT p.""Id"",
                                  p.""Title"",
                                  p.""ThumbnailUrl"",
                                  p.""Body"",
                                  p.""IsMature"", 
                                  p.""HashId"",
                                  p.""Type"",
                                  u.""ProfileName"",
                                  u.""UserName"",
                                  CASE 
                                  WHEN COUNT(t.""Name"") > 0 THEN array_agg(DISTINCT t.""Name"") 
                                  ELSE NULL 
                                  END AS Tags,
                                  COUNT(pc.""Id"") as CommentCount,
                                  to_jsonb(array_agg(sp.*)) AS ""SubPostStr""
                                  FROM ""story"".""StoryPosts""  p
                                  JOIN identity.""Users"" u ON  p.""CreatedBy""  = u.""Id"" 
                                  LEFT JOIN story.""StoryTagPosts"" tp on p.""Id""  = tp.""PostId"" 
                                  LEFT JOIN ""Tags"" t on t.""Id""  = tp.""TagId"" 
                                  LEFT JOIN ""story"".""StoryPostComments"" pc on pc.""PostId""  = p.""Id"" 
                                  LEFT JOIN LATERAL 
                                        (
                                            SELECT sp.""PostId"",sp.""Title"",sp.""Order"",sp.""CreatedOn""
                                            FROM ""story"".""StorySubPosts"" sp 
                                            WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false                                 
                                            GROUP BY sp.""Id"", sp.""PostId"", sp.""Title"",sp.""Order""
                                            ORDER BY sp.""Order"" DESC
                                            LIMIT 2
                                        ) sp ON sp.""PostId"" = p.""Id""    
                                  [QueryCondition]
                                  GROUP BY p.""Id"" ,u.""ProfileName"", u.""UserName""  
                                  ORDER BY p.""CreatedOn"" desc  
                                  OFFSET @Offset
                                  LIMIT @PageSize;

                                  SELECT COUNT(*) AS TotalCount
                                  FROM ""story"".""StoryPosts""  p
                                  JOIN identity.""Users"" u on p.""CreatedBy"" = u.""Id""
                                  [QueryCondition]";

        query = query.Replace("[QueryCondition]", queryCondition);
        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = input.PageSize,
                    Offset = offset,
                    PostStatus = (int)PostStatus.Public,
                    Permission = (int)PostPermission.Public,
                    ProfileName = input.Keyword
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostBoxResposne>(totalItems, input.PageNumber, input.PageSize);
            results.Items = MappingToPostBoxResponse(items);
            foreach (var item in results.Items)
            {
                item.Chapters = item.Chapters.DistinctBy(p => p.Order).ToList();
            }
        }
        else
        {
            results = new PagedResponse<PostBoxResposne>(0);
        }
        return results;
    }

    public async Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, int number)
    {
        ValidateTotalItem(number);
        var query = GetQuerySelectPage(PostSeriesSelectedType.RECOMMEND);

        var items = await _postRepository
                .Connection.QueryAsync<PostSeriesTopQueryDbResponse>(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = number,
                    PostStatus = (int)PostStatus.Public
                });

        return MappingTopSeries(items);
    }
    public async Task<PostSeriesResponse> UpdateSeries(string hashId, ComicPostUpdateSeriesR comicPostReq)
    {
        var ss = _currentUserService.Session;
        var currentUserId = ss.UserId;
        var profileId = ss.ProfileId;
        var currentFullName = ss.ProfileName;

        VerifyBasicInfo(comicPostReq.Title);

        #region Get post
        var query = string.Format(GetPostWithHashId, _postRepository.TableName);
        var post = await _postRepository.Connection.QueryFirstAsync<StoryPost>
            (query, new { HashId = hashId });
        VerifyPost(post, false);
        #endregion

        if (string.IsNullOrEmpty(comicPostReq.Summary))
        {
            throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
        }

        var currentTitle = post.Title;
        post.Title = comicPostReq.Title;
        post.HashId = hashId;
        post.AuthorId = comicPostReq.IsCurrentUserIsAuthor ? currentUserId : null;
        post.AuthorName = comicPostReq.IsCurrentUserIsAuthor ? currentFullName : comicPostReq.AuthorName;
        post.Body = comicPostReq.Summary;
        post.ThumbnailUrl = comicPostReq.ThumbnailUrl;
        post.CoverUrl = comicPostReq.CoverUrl;
        post.IsMature = comicPostReq.IsMature;
        post.Permission = comicPostReq.Permission;
        post.Status = comicPostReq.IsSaveAndPublish ? PostStatus.Public : PostStatus.Draft;
        post.IsCompleted = comicPostReq.IsCompleted;

        var result = new PostSeriesResponse
        {
            Id = post.Id,
            Title = comicPostReq.Title,
            HashId = hashId,
            UserId = currentUserId,
            Type = post.Type,
            ThumbnailUrl = post.ThumbnailUrl,
            CreatedOn = post.CreatedOn,
            Status = post.Status,
            Body = comicPostReq.Summary,
            CoverUrl = comicPostReq.CoverUrl,
            IsMature = comicPostReq.IsMature,
            Permission = comicPostReq.Permission,
            ProfileId = profileId,
            AuthorName = post.AuthorName,
            IsCurrentUserIsAuthor = comicPostReq.IsCurrentUserIsAuthor,
            IsCompleted = comicPostReq.IsCompleted
        };
        try
        {
            await _postRepository.UpdateAsync(post);

            if (currentTitle != comicPostReq.Title)
            {
                var currentEntity = await _context.SmartLookupAvailable.FirstOrDefaultAsync(p => p.KeywordType == LookupKeywordType.Story && p.Keyword == currentTitle);
                if (currentEntity != null)
                {
                    currentEntity.Keyword = comicPostReq.Title;
                    await _smartLookupRepository.UpdateAsync(currentEntity);
                }
            }

            result.Tags = (await _tagService.UpdateTagsToPost(post.Id, comicPostReq.Tags, currentUserId)).ToArray();
        }
        catch (Exception e)
        {
            _unitOfWork.RollbackTransaction();
            throw new Exception(e.ToString());
        }

        return result;

    }
    private PostSeriesResponse MappingFeedRespone(PostSeriesQueryDbResponse item)
    {
        var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;
        if (item == null)
            return new PostSeriesResponse();
        var totalChapterView = item.Chapters.Select(x => x.ViewCount).Sum();

        var freeChapters = item.Chapters.Where(x => x.Permission == PostPermission.Public).Count();
        var exclusiveChapters = item.Chapters.Where(x => x.UserExclusiveId.HasValue).Count();
        var totalChapters = item.Chapters.Count;
        var estimateBuyChapters = totalChapters - freeChapters - exclusiveChapters;

        var itemResponse = new PostSeriesResponse()
        {
            Id = item.Id,
            Title = item.Title,
            HashId = item.HashId,
            UserId = item.UserId,
            AuthorName = item.AuthorName,
            IsCurrentUserIsAuthor = currentUserId == item.AuthorId,
            ThumbnailUrl = item.ThumbnailUrl,
            CoverUrl = item.CoverUrl,
            CreatedOn = item.CreatedOn,
            ProfileId = item.ProfileId,
            UserName = item.UserName,
            ProfileName = item.ProfileName,
            UserAvatar = item.UserAvatar,
            Type = item.Type,
            IsMature = item.IsMature,
            IsCompleted = item.IsCompleted,
            Permission = item.Permission,
            Status = item.Status,
            Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
            Body = item.Body,
            Chapters = item.Chapters,
            ChapterCount = totalChapters,
            ViewCount = item.ViewCount + totalChapterView,
            TotalChapters = new ChaptersExclusiveData() { Count = totalChapters, Amount = totalChapters * Default.ChapterPrice },
            FreeChapters = new ChaptersExclusiveData() { Count = freeChapters, Amount = freeChapters * Default.ChapterPrice },
            ExclusiveChapters = new ChaptersExclusiveData() { Count = exclusiveChapters, Amount = exclusiveChapters * Default.ChapterPrice },
            EstimateBuyChapters = new ChaptersExclusiveData() { Count = estimateBuyChapters, Amount = estimateBuyChapters * Default.ChapterPrice },
            SeriesStatus = item.ToSeriesStatus(),
            TotalComment = item.TotalComment,
            IsFollowing = item.IsFollowing
        };

        return itemResponse;
    }

    public async Task<bool> FollowPost(Guid postId)
    {
        var currentUserId = _currentUserService?.Session?.UserId;
        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == currentUserId);
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        if (!await _context.StoryPostAvailable.AnyAsync(p => p.Id == postId))
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var followedPost = await _context.StoryPostFavorites
                                                        .Where(p => p.CreatedBy == user.Id && p.PostId == postId)
                                                        .FirstOrDefaultAsync();
        if (followedPost == null)
        {
            await _context.StoryPostFavorites.AddAsync(new StoryPostFavorite
            {
                UserId = user.Id,
                CreatedBy = user.Id,
                PostId = postId,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                ModifiedBy = user.Id,
            });
            await _context.SaveChangesAsync(default);
            return true;
        }
        else
        {
            followedPost.IsDelete = !followedPost.IsDelete;
            _context.StoryPostFavorites.Update(followedPost);
            await _context.SaveChangesAsync(default);
            return !followedPost.IsDelete;
        }
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(BasePageResultR loadReq)
    {

        ValidateTotalItem(loadReq.PageSize);
        var currentUserId = _currentUserService?.Session?.UserId;
        PagedResponse<PostSeriesTopResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(ComicPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostFollowedIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostFollowedCountQuery);

        var result = new PostSeriesAllTopResponse();

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", topSelectPostIdQuery)
            .Replace("[CountResults]", countTopQuery)
            .Replace("[OrderBy]", loadReq.OrderBy)
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery);


        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    IsAccessPrivate = false,
                    UserId = currentUserId,
                    PageSize = loadReq.PageSize,
                    Offet = offset
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, ComicPostListSeriesR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        var currentUserId = _currentUserService?.Session?.UserId;
        PagedResponse<PostSeriesTopResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(StoryPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostCountQuery);

        var result = new PostSeriesAllTopResponse();

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", topSelectPostIdQuery)
            .Replace("[CountResults]", countTopQuery)
            .Replace("[OrderBy]", loadReq.OrderBy)
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery);


        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    UserId = currentUserId,
                    PageSize = loadReq.PageSize,
                    Offet = offset
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
            var query = string.Format(GetMyAllComicStoryQuery);

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

            var query = GetLatestPostsByTypeQuery;
            query = query.Replace("[GetTotalCount]", GetCountPostByTypeQuery);

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

        var query = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",pc.""CreatedOn"",p.""Type"" , 
                    p.""HashId"" as HashPostId,
                    FALSE as IsSubPost , 
                    NULL as Order
                    from ""story"".""StoryPostComments"" pc 
                    left join ""story"".""StoryPosts""  p on  pc.""PostId"" = p.""Id""
                    left join ""identity"".""Users"" u on pc.""CreatedBy"" = u.""Id""
                    WHERE pc.""CreatedBy"" = ANY(@UserIds)
                    AND pc.""CreatedBy"" != @CurrentUserId
                    AND pc.""CreatedOn"" < now()
                    AND pc.""CreatedOn"" > @FromDate
                    AND p.""IsDelete"" = false
                    AND pc.""IsDelete"" = false
                    UNION 
                    select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",spc.""Body"",spc.""PostId"",spc.""Id"",spc.""CreatedOn"",p.""Type"", 
                    p.""HashId"" as HashPostId,
                    TRUE as IsSubPost, 
                    sp.""Order""
                    from ""story"".""StorySubPostComments"" spc 
                    left join ""story"".""StorySubPosts"" sp on  spc.""PostId"" = sp.""Id""
                    LEFT JOIN ""story"".""StoryPosts""  p on p.""Id"" = sp.""PostId""
                    left join ""identity"".""Users"" u on spc.""CreatedBy"" = u.""Id""
                    WHERE spc.""CreatedBy"" = ANY(@UserIds)
                    AND spc.""CreatedBy"" != @CurrentUserId
                    AND spc.""CreatedOn"" < now()
                    AND spc.""CreatedOn"" > @FromDate
                    AND p.""IsDelete"" = false
                    AND sp.""IsDelete"" = false
                    AND spc.""IsDelete"" = false
                    Order by ""CreatedOn"" desc
                    LIMIT 2";

        var data = await _postCommentRepository.Connection.QueryAsync<NewsFeedDto>(query, new
        {
            FromDate = DateTime.Today,
            UserIds = userFollowingIds,
            CurrentUserId = user.Id
        });
        var amountDataNeedToTake = data != null ? input.PageSize - data.Count() : input.PageSize;
        var queryDataNeedToTake = @"select u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",pc.""CreatedOn"",p.""Type"" , 
                        p.""HashId"" as HashPostId,
                        FALSE as IsSubPost, NULL as Order,
                        COALESCE(COUNT(pcr.""Id""), 0) AS reaction_count,
                         RANDOM() AS sort_key
                        FROM ""story"".""StoryPostComments"" pc 
                        LEFT JOIN ""story"".""StoryPosts""  p on  pc.""PostId"" = p.""Id""
                        LEFT JOIN ""story"".""StoryPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
                        LEFT JOIN ""identity"".""Users"" u on pc.""CreatedBy"" = u.""Id""
                        WHERE pc.""Id"" <> ALL (ARRAY[@CommentIds]) 
                        AND pc.""CreatedBy"" != @CurrentUserId
                        AND p.""IsDelete"" = false
                        AND pc.""IsDelete"" = false
                        GROUP BY p.""HashId"", u.""Avatar"",u.""UserName"",u.""ProfileName"",pc.""Body"",pc.""PostId"",pc.""Id"",p.""Type""
                        UNION 
                        SELECT u.""Avatar"" as UserAvatar,u.""UserName"",u.""ProfileName"",spc.""Body"",spc.""PostId"",spc.""Id"",spc.""CreatedOn"",p.""Type"",
                        p.""HashId"" as HashPostId,
                        TRUE as IsSubPost, sp.""Order"",
                        COALESCE(COUNT(spcr.""Id""), 0) AS reaction_count,
                        RANDOM() AS sort_key
                        FROM ""story"".""StorySubPostComments"" spc 
                        LEFT join ""story"".""StorySubPosts"" sp on  spc.""PostId"" = sp.""Id""
                        LEFT JOIN ""story"".""StorySubPostCommentReactions""  spcr ON spc.""Id"" = spcr.""TargetId""
                        LEFT JOIN ""story"".""StoryPosts""  p on p.""Id"" = sp.""PostId""
                        LEFT join ""identity"".""Users"" u on spc.""CreatedBy"" = u.""Id""
                        WHERE spc.""Id"" <> ALL (ARRAY[@CommentIds])     
                        AND spc.""CreatedBy"" != @CurrentUserId
                        AND p.""IsDelete"" = false
                        AND sp.""IsDelete"" = false
                        AND spc.""IsDelete"" = false
                        GROUP BY p.""HashId"",u.""Avatar"",u.""UserName"",u.""ProfileName"",spc.""Body"",spc.""PostId"",spc.""Id"",spc.""CreatedOn"",p.""Type"",sp.""Order""
                        ORDER BY sort_key
                        LIMIT @Limit";

        var commentIds = data.Select(p => p.Id).ToArray();
        var dataNeedToTake = await _postCommentRepository.Connection.QueryAsync<NewsFeedDto>(queryDataNeedToTake, new
        {
            CommentIds = amountDataNeedToTake < input.PageSize ? commentIds : [],
            Limit = amountDataNeedToTake,
            CurrentUserId = user.Id
        });

        return data.Concat(dataNeedToTake).ToList();
    }

    public async Task<PagedResponse<RelatedBoxResponse>> GetPostMaybeYouLike(UserNamePagingR input)
    {

        var currentUserId = await _context.UserAvailable.AsNoTracking()
                                                .Where(p => p.UserName == input.UserName)
                                                .Select(p => p.Id)
                                                .FirstOrDefaultAsync();

        var offset = input.PageSize * (input.PageNumber - 1);
        var postIdReaded = new List<Guid> { Guid.Empty }; //TODO Analytic
        if (postIdReaded.Any())
        {
            var tagIds = await _postReportRepository.Connection.QueryAsync<Guid>($@"select DISTINCT tp.""TagId"" 
                                                                                            from story.""StoryTagPosts"" tp 
                                                                                            join ""story"".""StoryPosts""  p on tp.""PostId"" =  p.""Id""
                                                                                            WHERE tp.""PostId"" = ANY (@PostId)
                                                                                            AND tp.""IsDelete"" = false
                                                                                             ", new { PostId = postIdReaded });
            var query = GetRelatedBoxPostQuery;
            query = query.Replace("[QueryCondition]", @"AND t.""Id"" = ANY(@TagIds)");
            var dataQuery = await _postReportRepository.Connection.QueryAsync<RelatedBoxQueryResponse>(query, new
            {
                Limit = input.PageSize,
                TagIds = tagIds,
            });
            var items = MappingRelatedBoxResponse(dataQuery);
            if (items != null && items.Count() > 0)
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
        /// haven't read any stories/comics yet
        else
        {
            var query = GetRelatedBoxPostQuery;
            query = query.Replace("[QueryCondition]", "");
            var dataQuery = await _postReportRepository.Connection.QueryAsync<RelatedBoxQueryResponse>(query, new
            {
                Limit = input.PageSize,
            });
            var items = MappingRelatedBoxResponse(dataQuery);
            if (items != null && items.Count() > 0)
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
                    ThumbnailUrl = res.ThumbnailUrl,
                    Body = res.Body,
                    Title = res.Title,
                    AuthorId = res.AuthorId,
                    AuthorName = res.AuthorName,
                    ViewCount = res.ViewCount,
                    Tags = res.Tags != null ? JsonConvert.DeserializeObject<List<string>>(res.Tags.ToString()) : new List<string>(),
                    Chapters = chapters,
                    ChapterCount = chapters?.Count ?? 0,
                    Type = res.Type,
                    HashId = res.HashId,
                    CreatedOn = res.CreatedOn,
                    ProfileName = res.ProfileName,
                    UserId = res.UserId
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

    public UploadFileDto MappingFile(StoryResource resources)
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
            Url = _setting.Minio.MediaApiUrl.GetMediaPath(resources.Name, resources.Url),
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
            TotalComment = x.TotalComment,
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
        return (PublishDate != null && PublishDate < DateTime.Now);
    }

    #endregion

    #region Chapters        
    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR loadReq)
    {
        PagedResponse<ChapterResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(StorySubPost.Order);
        }
        var query = GetSeriesChaptersByHashId
            .Replace("[OrderBy]", loadReq.OrderBy);

        //TODO Premium
        var currentUserId = _currentUserService?.Session?.UserId;
        query = AddWithPermission(query, currentUserId);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    IsAccessPrivate = false,
                    PostHashId = hashId,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    UserId = currentUserId
                });
        var items = await multi.ReadAsync<ChapterResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<ChapterResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
            foreach (var item in items)
            {
                item.IsPublicNow = CheckIsPublicNow(item.PublishDate);
                item.Body = System.Web.HttpUtility.HtmlDecode(item.Body);
            }
            results.Items = items;
        }
        else
        {
            results = new PagedResponse<ChapterResponse>(0);
        }
        return results;
    }
    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
    {
        PagedResponse<ChapterTOCResponse> results;
        var query = GetSeriesChaptersSimpleByHashId;

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostHashId = hashId
                });
        var items = await multi.ReadAsync<ChapterTOCResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<ChapterTOCResponse>(totalItems, 1, totalItems);
            results.Items = items;
        }
        else
        {
            results = new PagedResponse<ChapterTOCResponse>(0);
        }
        return results;
    }
    public async Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ComicChapterListR loadReq)
    {
        PagedResponse<ChapterTOCExtendResponse> results;
        var query = GetSeriesChaptersWithOffsetSimpleByHashId;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(StorySubPost.Order);
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

    public async Task<StorySubPost> SubPostChapterToSeries(string comicHashId, StoryChapterPostR chapterPostReq)
    {
        var currentUserId = _currentUserService?.Session?.UserId;
        if (!chapterPostReq.IsPublicNow && chapterPostReq.PublishDate == null)
        {
            throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
        }
        #region Get post
        var newOrder = 0.0f;
        var query = string.Format(GetPostAndLastSubPostOrder, _postRepository.TableName);
        var reader = await _postRepository
            .Connection.QueryMultipleAsync(query, new { HashId = comicHashId });
        var post = (await reader.ReadAsync<StoryPost>().ConfigureAwait(false)).FirstOrDefault();
        VerifyPost(post, true);
        #endregion

        if (await _context.StorySubPostAvailable.AnyAsync(p => p.PostId == post.Id && p.Order == chapterPostReq.Order))
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }

        if (chapterPostReq.IsAutoGenerateOrder)
        {
            var maxOrder = (await reader.ReadAsync<float>(false)).FirstOrDefault();
            newOrder = (int)maxOrder + 1;

        }
        else
        {
            newOrder = chapterPostReq.Order.HasValue ? chapterPostReq.Order.Value : 0;
        }

        reader.Dispose();

        var newChapter = new StorySubPost
        {
            AuthorId = post.AuthorId,
            CreatedBy = currentUserId,
            Permission = chapterPostReq.Permission,
            Order = newOrder,
            Name = string.IsNullOrEmpty(chapterPostReq.Name) ? newOrder.ToString() : chapterPostReq.Name,
            PostId = post.Id,
            Status = PostStatus.Public,
            PublishDate = chapterPostReq.IsPublicNow ? DateTime.UtcNow : TimeZoneInfo.ConvertTimeToUtc(chapterPostReq.PublishDate ?? DateTime.Now),
            Title = chapterPostReq.Title,
            UserId = currentUserId ?? Guid.Empty,
            //CreatorNote = chapterPostReq.CreatorNote,
            IsEnableComment = chapterPostReq.IsEnableComment,
            ViewCount = 0,
            HashId = PostConfig.SubHashLength.GetRandomString(),
            IsExclusive = false, //BCW-37
            IsPremium = chapterPostReq.IsPremium,
        };
        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = currentUserId;
        await _postRepository.UpdateAsync(post);

        return newChapter;
    }
    public async Task<StorySubPost> SubPostUpdateChapterToSeries(string postHashId, float order, StoryChapterPostR chapterPostReq)
    {
        var currentUserId = _currentUserService?.Session?.UserId;
        if (!chapterPostReq.IsPublicNow && chapterPostReq.PublishDate == null)
        {
            throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
        }
        #region Get post
        var query = string.Format(GetPostWithHashId, _postRepository.TableName);
        var post = await _postRepository
            .Connection.QueryFirstAsync<StoryPost>(query, new { HashId = postHashId });
        VerifyPost(post, false);
        #endregion

        //Getchapter
        var querySubpost = string.Format(GetSeriesChapterByHashIdOrder, _postRepository.TableName);

        var newChapter = await _subPostRepository
            .Connection.QueryFirstAsync<StorySubPost>(querySubpost, new
            {
                PostHashId = postHashId,
                IsAccessPrivate = false,
                SubPostOrder = order,
            });

        if (await _context.ComicSubPostAvailable.AnyAsync(p => p.PostId == post.Id && p.Order == chapterPostReq.Order && p.Id != newChapter.Id))
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }

        newChapter.PublishDate = chapterPostReq.IsPublicNow ? DateTime.UtcNow : TimeZoneInfo.ConvertTimeToUtc(chapterPostReq.PublishDate ?? DateTime.Now);
        newChapter.Title = chapterPostReq.Title;
        if (!string.IsNullOrEmpty(chapterPostReq.Name))
        {
            newChapter.Name = chapterPostReq.Name;
        }

        newChapter.ModifiedOn = DateTime.UtcNow;
        newChapter.ModifiedBy = currentUserId;
        //newChapter.CreatorNote = chapterPostReq.CreatorNote;
        newChapter.IsEnableComment = chapterPostReq.IsEnableComment;
        newChapter.Permission = chapterPostReq.Permission;
        newChapter.IsPremium = chapterPostReq.IsPremium;
        newChapter.Order = chapterPostReq.Order.HasValue ? chapterPostReq.Order.Value : order;
        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = currentUserId;

        await _postRepository.UpdateAsync(post);

        return newChapter;
    }
    public async Task<bool> DeleteChapter(string hashId, int order)
    {
        var ss = _currentUserService.Session;
        var currentUserId = ss.UserId;
        var profileName = ss.ProfileName;

        var subPost = await _postRepository.Connection.QueryFirstAsync<StorySubPost>(GetSubPostIdWithHashIdAndOrder, new { HashId = hashId, Order = order });
        if (subPost == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, order));
        }
        else if (subPost.UserId != currentUserId)
        {
            throw new BadRequestException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);

        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeleteSubPost, new { SubPostId = subPost.Id, Date = DateTime.UtcNow, UserId = currentUserId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(subPost.PostId, profileName);
            return true;
        }
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
    public ChapterResponse MappingChapterResponse(StorySubPost newChapter)
    {
        var result = new ChapterResponse();

        result.Id = newChapter.Id;
        result.HashId = newChapter.HashId;
        result.Body = System.Web.HttpUtility.HtmlDecode(newChapter.Body);
        result.ViewCount = newChapter.ViewCount;
        result.Permission = newChapter.Permission;
        result.Order = newChapter.Order;
        result.PostId = newChapter.PostId;
        result.Status = newChapter.Status;
        result.Title = newChapter.Title;
        result.Name = newChapter.Name;
        result.CreatedOn = newChapter.CreatedOn;
        result.CreatorNote = newChapter.CreatorNote;
        result.PublishDate = newChapter.PublishDate;
        result.CreatedBy = newChapter.CreatedBy;
        result.IsPublicNow = CheckIsPublicNow(newChapter.PublishDate);
        result.IsEnableComment = newChapter.IsEnableComment;
        result.IsExclusive = newChapter.IsExclusive;
        //When new, return 0
        result.CommentCount = 0;
        return result;
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string postHashId, ComicChapterOrderSwapR orders)
    {
        var result = new List<ChapterResponse>();
        var subPosts = await _postRepository
            .Connection.QueryAsync<StorySubPost>(GetSubPostsWithHashIdAndOrders, new { HashId = postHashId, Order1 = orders?.Order1, Order2 = orders?.Order2 });

        var currentUserId = _currentUserService.Session.UserId;
        var chapter1 = subPosts.Where(x => x.Order == orders?.Order1).FirstOrDefault();
        if (chapter1 == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, orders?.Order1));
        }
        else if (chapter1.UserId != currentUserId)
        {
            throw new BadRequestException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
        }

        try
        {
            var chapter2 = subPosts.Where(x => x.Order == orders?.Order2).FirstOrDefault();
            if (chapter2 != null)
            {
                chapter2.Order = orders?.Order1 ?? 0;
                await _subPostRepository.UpdateAsync(chapter2);
                result.Add(MappingChapterResponse(chapter2));
            }

            chapter1.Order = orders?.Order2 ?? 0;
            await _subPostRepository.UpdateAsync(chapter1);
            result.Add(MappingChapterResponse(chapter1));

        }
        catch (Exception)
        {
            _unitOfWork.RollbackTransaction();
            throw;
        }

        return result;
    }
    #endregion

    #region POST - COMMON
    private void VerifyPost(StoryPost post, bool checkCompleted)
    {
        var currentUserId = _currentUserService.Session.UserId;
        if (post != null)
        {
            if (post.IsDelete)
            {
                throw new BadRequestException(E205, M205);
            }
            if (post.CreatedBy != currentUserId)
            {
                //TODO check permission
                //throw new ForbiddenAccessException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
            }
            if (checkCompleted && (post.IsCompleted ?? false))
            {
                throw new ForbiddenAccessException(ApiErrorCode.POST_HAS_COMPLETED, ApiErrorMessage.POST_HAS_COMPLETED);
            }
        }
        else
        {
            throw new NotFoundException(E204, M204);
        }
    }

    public void VerifyBasicInfo(string title)
    {
        if (title.Length > 255)
            throw new BadRequestException(ApiErrorCode.POST_TITLE_LESS_THAN_CHARACTER, ApiErrorMessage.POST_TITLE_LESS_THAN_CHARACTER);
        //if (body.Length >= 2000)
        //{
        //    throw new BadRequestException(ApiErrorCode.POST_NOTE_LESS_THAN_CHARACTER, ApiErrorMessage.POST_NOTE_LESS_THAN_CHARACTER);
        //}
    }

    private int GetOffsetSetup(ref ComicTopPostR loadReq)
    {
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(StoryPost.CreatedOn);
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

        var check = await _context.StoryPostAvailable.FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
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
                               FROM ""story"".""StorySubPosts"" sp
                               JOIN ""story"".""StoryPosts""  p ON sp.""PostId"" = p.""Id""
                               JOIN ""story"".""StoryResources"" r on sp.""Id"" = r.""SubPostId""
                               WHERE p.""IsDelete"" = false
                               AND sp.""IsDelete"" = false
                               [QueryByType]
                               [IgnoreQuery]
                               AND sp.""Order"" = (
                                                    SELECT MIN(sp_inner.""Order"")
                                                    FROM ""story"".""StorySubPosts"" sp_inner
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
            var query = @$"SELECT ""HashId"" From ""story"".""StoryPosts""  
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

    #endregion
}
