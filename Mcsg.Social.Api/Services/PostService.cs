using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;
    using Common.SeedWork.Extensions;
    using Constants;
    using Enums;
    using Extensions;
    using Interfaces;
    using Lib.Common.Constants;
    using Lib.Common.Enums;
    using Lib.Common.Exceptions;
    using Lib.Common.Helpers;
    using Lib.Common.Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Models;
    using Models.Earning;
    using Requests;
    using static Common.Core.Constants.Setting;

    public partial class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<SmartLookup> _smartLookupRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<PostReport> _postReportRepository;
        private readonly IValidator<PostReport> _postReportValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagService _tagService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly FileSetting _fileSetting;
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
            IOptionsMonitor<FileSetting> fileSetting,
            ICurrentUserService currentUserService,
            IViewHistoryService viewHistoryService,
            IConfiguration configuration,
            IMapper mapper,
            ISetting setting,
            ISmartLookupService smartLookupService,
            IValidator<PostReport> postReportValidator,
            IRepository<PostComment> postCommentRepository)
        {
            _postRepository = unitOfWork.GetRepository<Post>();
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _postReportRepository = unitOfWork.GetRepository<PostReport>();
            _unitOfWork = unitOfWork;
            _tagService = tagService;
            _userService = userService;
            _fileService = fileService;
            _fileSetting = fileSetting.CurrentValue;
            _currentUserService = currentUserService;
            _viewHistoryService = viewHistoryService;
            _smartLookupService = smartLookupService;
            _configuration = configuration;
            _mapper = mapper;
            _setting = setting;
            _postReportValidator = postReportValidator;
            _smartLookupRepository = smartLookupRepository;
            _postCommentRepository = postCommentRepository;
        }
        public async Task<bool> Delete(Guid postId)
        {
            var feedDb = await _postRepository.GetByIdAsync(postId);
            var currentUserId = _currentUserService.Session.UserId;
            if (feedDb == null)
            {
                throw new BadRequestException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }
            else if (feedDb.UserId != currentUserId)
            {
                throw new BadRequestException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
            }
            else
            {
                await _postRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = DateTime.UtcNow, UserId = currentUserId });

                await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(postId);
                return true;
            }
        }

        #region PostStoryOrComic
        public async Task<PostSeriesResponse> PostSeries(PostType type, PostSeriesReq comicPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var profileId = _currentUserService.Session.ProfileId;
            var currentFullName = $"{_currentUserService.Session.ProfileName}";
            VerifyBasicInfo(comicPostReq.Title);
            //Check first post
            var rewards = await CheckRewardsForPost(currentUserId, type);

            var hashId = SystemConfig.PostHashLength.GetRandomString();
            //var safePlainString = "";
            //if (!string.IsNullOrEmpty(comicPostReq.Summary))
            //{
            //    safePlainString = System.Web.HttpUtility.HtmlEncode(comicPostReq.Summary);
            //}

            var post = new Post()
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
                CreatedDate = post.CreatedDate,
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
                if (comicPostReq.Tags != null && comicPostReq.Tags.Count > 0)
                {
                    result.Tags = (await _tagService.AddTagsToPost(post.Id, comicPostReq.Tags)).ToArray();
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
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }
            if (currentUserId != null)
            {
                //await _viewHistoryService.QueueAddView(currentUserId ?? Guid.Empty, dbPost.Id, EntityType.POST, "", (EntitySubType)(dbPost.Type));
            }
            if (dbPost.Status == PostStatus.Inactive || (dbPost.Status == PostStatus.Draft && dbPost.UserId != currentUserId))
            {
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }
            dbPost.TotalComment = await _postRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = hashId });
            return MappingFeedRespone(dbPost);
        }
        public async Task<ChapterResponse> GetSeriesChapter(string hashId, int order)
        {
            var query = string.Format(GetSeriesChapterByHashIdWithJoinOrder, _postRepository.TableName);
            var userIsPremium = _currentUserService?.Session?.IsPremium ?? false;
            var currentUserId = _currentUserService?.Session?.UserId;

            ChapterResponse subpost = null;
            await _subPostRepository
                .Connection.QueryAsync<ChapterResponse, Resource, ChapterResponse>(query,
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
                            subpost.Files = new List<UploadFileResponse>();
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
                        throw new BadRequestException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
                    }
                    //Check permission
                    if (subpost.Permission == PostPermission.PRIVATE)
                    {
                        throw new BadRequestException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
                    }
                    //Check IsExclusive
                    if (subpost.IsExclusive && subpost.UserExclusiveId == null)
                    {
                        throw new BadRequestException(ApiErrorCode.NEED_BUY_TO_READ, ApiErrorMessage.NEED_BUY_TO_READ);
                    }
                    if (subpost.Permission == PostPermission.PREMIUM && (!userIsPremium && subpost.UserExclusiveId == null))
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
                .Replace("[OrderBy]", "CreatedDate");

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
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, PostListSeriesReq request)
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
                .Replace("[OrderBy]", "CreatedDate");

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = type,
                        PageSize = request.PageSize,
                        Offet = offset,
                        LastWeek = (DateTime.UtcNow.AddDays(-7)),
                        PostStatus = (int)PostStatus.Public,
                        TagName = request.HashTag,
                        UserId = currentUserId
                    });

            var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var items = MapTopSeries(dbFeed.ToList());
            if (items != null && items.Count() > 0)
            {
                var results = new PagedResults<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
                results.Items = items;
                return results;
            }
            else
            {
                return new PagedResults<PostSeriesTopResponse>(0);
            }
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, RelationPostSeriesReq request)
        {
            try
            {
                #region Get post
                var queryGetPost = string.Format(GetPostWithHashId, _postRepository.TableName);
                var post = await _postRepository.Connection.QueryFirstAsync<Post>(queryGetPost, new { HashId = request.HashId });
                #endregion

                if (post == null)
                {
                    throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
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

                var query = GetTopAllPostAllTypeByTagQuery.Replace("[SelectPostIdsQuery]", allSubQuery)
                    .Replace("[WhereMainQuery]", whereClause)
                    .Replace("[CountResults]", countTopQuery)
                    .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
                    .Replace("[OrderBy]", "CreatedDate");

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
                    var results = new PagedResults<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
                    results.Items = items;
                    return results;
                }
                else
                {
                    return new PagedResults<PostSeriesTopResponse>(0);
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, TopPostReq loadReq)
        {
            ValidateTotalItem(loadReq.PageSize);
            PagedResults<PostSeriesTopResponse> results;
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
                results = new PagedResults<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                results.Items = MappingTopSeries(items);
            }
            else
            {
                results = new PagedResults<PostSeriesTopResponse>(0);
            }
            return results;
        }

        public async Task UpdateKeyWordForComicAndStoryToSmartLookup()
        {
            var queryNameListPost = $@"SELECT ""Title"" FROM ""Posts"" where ""Type"" != {(int)PostType.Feed}  AND ""IsDelete"" = false ";
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

        public async Task<PagedResults<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, TopPostReq loadReq)
        {
            ValidateTotalItem(loadReq.PageSize);
            PagedResults<PostSeriesTopResponse> results;
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
                results = new PagedResults<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                results.Items = MappingTopSeries(items);
            }
            else
            {
                results = new PagedResults<PostSeriesTopResponse>(0);
            }
            return results;
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, TopPostReq loadReq)
        {
            ValidateTotalItem(loadReq.PageSize);
            PagedResults<PostSeriesTopResponse> results;
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
                results = new PagedResults<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                results.Items = MappingTopSeries(items);
            }
            else
            {
                results = new PagedResults<PostSeriesTopResponse>(0);
            }
            return results;
        }

        public async Task<PagedResults<PostBoxResposne>> GetPostByTagName(PostType type, PostByTagNameInput input)
        {
            ValidateTotalItem(input.PageSize);
            PagedResults<PostBoxResposne> results;
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
                results = new PagedResults<PostBoxResposne>(totalItems, input.PageNumber, input.PageSize);
                results.Items = MappingToPostBoxResponse(items);
            }
            else
            {
                results = new PagedResults<PostBoxResposne>(0);
            }
            return results;
        }

        public async Task<PagedResults<PostBoxResposne>> GetPostByUserProfileName(PostType type, PostByProFileNameInput input)
        {
            ValidateTotalItem(input.PageSize);
            PagedResults<PostBoxResposne> results;
            var offset = input.PageSize * (input.PageNumber - 1);

            var queryCondition = "";
            if (input.SearchBy == "ProfileName")
            {
                queryCondition = $@"WHERE u.""ProfileName""=@ProfileName 
                                   AND p.""Type""=@PostType
                                   AND p.""Status""=@PostStatus
                                   AND p.""IsDelete""=false";
            }
            else
            {
                queryCondition = $@" WHERE p.""Title"" ILIKE '%{input.Keyword}%'
                                     AND p.""Type""=@PostType
                                     AND p.""Status""=@PostStatus
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
	                              CASE 
	                              WHEN COUNT(t.""Name"") > 0 THEN array_agg(DISTINCT t.""Name"") 
	                              ELSE NULL 
	                              END AS Tags,
	                              COUNT(pc.""Id"") as CommentCount,
	                              to_jsonb(array_agg(sp.*)) AS ""SubPostStr""
	                              FROM ""Posts"" p
	                              JOIN identity.""Users"" u ON  p.""CreatedBy""  = u.""Id"" 
	                              LEFT JOIN ""TagPosts"" tp on p.""Id""  = tp.""PostId"" 
	                              LEFT JOIN ""Tags"" t on t.""Id""  = tp.""TagId"" 
	                              LEFT JOIN ""PostComments"" pc on pc.""PostId""  = p.""Id"" 
	                              LEFT JOIN LATERAL 
										(
											SELECT sp.""PostId"",sp.""Title"",sp.""Order"",sp.""CreatedDate""
											FROM ""SubPosts"" sp 
											WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false 								
											GROUP BY sp.""Id"", sp.""PostId"", sp.""Title"",sp.""Order""
											ORDER BY sp.""Order"" DESC
											LIMIT 2
										) sp ON sp.""PostId"" = p.""Id""	
                                  [QueryCondition]
                                  GROUP BY p.""Id"" ,u.""ProfileName"" 
                                  ORDER BY p.""CreatedDate"" desc  
                                  OFFSET @Offset
                                  LIMIT @PageSize;

                                  SELECT COUNT(*) AS TotalCount
                                  FROM ""Posts"" p
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
                        ProfileName = input.Keyword
                    });
            var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResults<PostBoxResposne>(totalItems, input.PageNumber, input.PageSize);
                results.Items = MappingToPostBoxResponse(items);
                foreach (var item in results.Items)
                {
                    item.Chapters = item.Chapters.DistinctBy(p => p.Order).ToList();
                }
            }
            else
            {
                results = new PagedResults<PostBoxResposne>(0);
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
        public async Task<PostSeriesResponse> UpdateSeries(string hashId, PostUpdateSeriesReq comicPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var currentFullName = $"{_currentUserService.Session.FirstName} {_currentUserService.Session.LastName}";
            var profileId = _currentUserService.Session.ProfileId;
            VerifyBasicInfo(comicPostReq.Title);

            #region Get post
            var query = string.Format(GetPostWithHashId, _postRepository.TableName);
            var post = await _postRepository.Connection.QueryFirstAsync<Post>
                (query, new { HashId = hashId });
            VerifyPost(post, false);
            #endregion

            if (string.IsNullOrEmpty(comicPostReq.Summary))
            {
                throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
            }

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
                CreatedDate = post.CreatedDate,
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
                if (comicPostReq.Tags != null && comicPostReq.Tags.Count > 0)
                {
                    result.Tags = (await _tagService.UpdateTagsToPost(post.Id, comicPostReq.Tags)).ToArray();
                }
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

            var freeChapters = item.Chapters.Where(x => x.Permission == PostPermission.PUBLIC).Count();
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
                CreatedDate = item.CreatedDate,
                ProfileId = item.ProfileId,
                ProfileName = item.ProfileName,
                UserAvatar = string.IsNullOrEmpty(item.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.UserAvatar),
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
            };

            return itemResponse;
        }
        public async Task<PagedResults<PostSeriesTopResponse>> GetMySeries(PostType type, PostListSeriesReq loadReq)
        {
            ValidateTotalItem(loadReq.PageSize);
            var currentUserId = _currentUserService?.Session?.UserId;
            PagedResults<PostSeriesTopResponse> results;
            var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

            if (loadReq.OrderBy == null)
            {
                loadReq.OrderBy = nameof(Post.CreatedDate);
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
                results = new PagedResults<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                results.Items = MappingTopSeries(items);
            }
            else
            {
                results = new PagedResults<PostSeriesTopResponse>(0);
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
                var listItemResponse = new List<FeedResponse>();

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
                        ChapterCount = res.ChapterCount,
                        Type = res.Type,
                        HashId = res.HashId,
                        CreatedDate = res.CreatedDate,

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

        public UploadFileResponse MappingFile(Resource resources)
        {
            if (resources == null || resources.Id == Guid.Empty)
            {
                return null;
            }
            return new UploadFileResponse
            {
                HashId = resources.HashId,
                Order = resources.Order,
                Name = resources.Name,
                Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, resources.Name, resources.Url),
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
                UserAvatar = string.IsNullOrEmpty(x.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, x.UserAvatar),
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
                CreatedDate = x.CreatedDate,
                Id = x.Id,
                IsMature = x.IsMature,
                IsCompleted = x.IsCompleted,
                Permission = x.Permission,
                Status = x.Status,
                UserId = x.UserId,
                //"AuthorName", "CoverUrl","CreatedDate", "IsMature", "Id", "Permission", "Status", "UserId"
                HashId = x.HashId,
                Chapters = MappingTopChapter(x.SubPostStr),
            }).ToList();
        }

        private List<PostBoxResposne> MappingToPostBoxResponse(IEnumerable<PostSeriesTopQueryDbResponse> posts)
        {
            return posts.Select(x => new PostBoxResposne
            {
                ProfileName = x.ProfileName,
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
                UserAvatar = string.IsNullOrEmpty(x.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, x.UserAvatar),
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
                CreatedDate = x.CreatedDate,
                Id = x.Id,
                IsMature = x.IsMature,
                IsCompleted = x.IsCompleted,
                Permission = x.Permission,
                Status = x.Status,
                UserId = x.UserId,
                HashId = x.HashId,
                Chapters = MappingTopChapter(x.SubPostStr),
                SeriesStatus = x.ToSeriesStatus()
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
                .Replace("[OrderBy]", "CreatedDate");

            return query;
        }

        private bool CheckIsPublicNow(DateTime? PublishDate)
        {
            return (PublishDate != null && PublishDate < DateTime.UtcNow);
        }

        #endregion

        #region Chapters        
        public async Task<PagedResults<ChapterResponse>> GetChapters(string hashId, ChapterListReq loadReq)
        {
            PagedResults<ChapterResponse> results;
            var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

            if (loadReq.OrderBy == null)
            {
                loadReq.OrderBy = nameof(SubPost.Order);
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
                results = new PagedResults<ChapterResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                foreach (var item in items)
                {
                    item.IsPublicNow = CheckIsPublicNow(item.PublishDate);
                    item.Body = System.Web.HttpUtility.HtmlDecode(item.Body);
                }
                results.Items = items;
            }
            else
            {
                results = new PagedResults<ChapterResponse>(0);
            }
            return results;
        }
        public async Task<PagedResults<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
        {
            PagedResults<ChapterTOCResponse> results;
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
                results = new PagedResults<ChapterTOCResponse>(totalItems, 1, totalItems);
                results.Items = items;
            }
            else
            {
                results = new PagedResults<ChapterTOCResponse>(0);
            }
            return results;
        }
        public async Task<PagedResults<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, ChapterListReq loadReq)
        {
            PagedResults<ChapterTOCExtendResponse> results;
            var query = GetSeriesChaptersWithOffsetSimpleByHashId;
            var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

            if (loadReq.OrderBy == null)
            {
                loadReq.OrderBy = nameof(SubPost.Order);
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
                results = new PagedResults<ChapterTOCExtendResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
                results.Items = items;
            }
            else
            {
                results = new PagedResults<ChapterTOCExtendResponse>(0);
            }
            return results;
        }

        public async Task<SubPost> SubPostChapterToSeries(string comicHashId, ChapterPostReq chapterPostReq)
        {
            var currentUserId = _currentUserService?.Session?.UserId;
            if (!chapterPostReq.IsPublicNow && chapterPostReq.PublishDate == null)
            {
                throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
            }
            #region Get post
            var query = string.Format(GetPostAndLastSubPostOrder, _postRepository.TableName);
            var reader = await _postRepository
                .Connection.QueryMultipleAsync(query, new { HashId = comicHashId });
            var post = (await reader.ReadAsync<Post>().ConfigureAwait(false)).FirstOrDefault();
            var maxOrder = (await reader.ReadAsync<int>(false)).FirstOrDefault();
            reader.Dispose();
            VerifyPost(post, true);
            #endregion
            var newOrder = maxOrder + 1;
            var newChapter = new SubPost
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
                HashId = SystemConfig.SubPostHashLength.GetRandomString(),
                IsExclusive = false, //BCW-37
            };
            post.LastModifiedDate = DateTime.UtcNow;
            post.LastModifiedBy = currentUserId;
            await _postRepository.UpdateAsync(post);

            return newChapter;
        }
        public async Task<SubPost> SubPostUpdateChapterToSeries(string postHashId, int order, ChapterPostReq chapterPostReq)
        {
            var currentUserId = _currentUserService?.Session?.UserId;
            if (!chapterPostReq.IsPublicNow && chapterPostReq.PublishDate == null)
            {
                throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
            }
            #region Get post
            var query = string.Format(GetPostWithHashId, _postRepository.TableName);
            var post = await _postRepository
                .Connection.QueryFirstAsync<Post>(query, new { HashId = postHashId });
            VerifyPost(post, false);
            #endregion

            //Getchapter
            var querySubpost = string.Format(GetSeriesChapterByHashIdOrder, _postRepository.TableName);

            var newChapter = await _subPostRepository
                .Connection.QueryFirstAsync<SubPost>(querySubpost, new
                {
                    PostHashId = postHashId,
                    IsAccessPrivate = false,
                    SubPostOrder = order,
                });

            newChapter.PublishDate = chapterPostReq.IsPublicNow ? DateTime.UtcNow : TimeZoneInfo.ConvertTimeToUtc(chapterPostReq.PublishDate ?? DateTime.Now);
            newChapter.Title = chapterPostReq.Title;
            if (!string.IsNullOrEmpty(chapterPostReq.Name))
            {
                newChapter.Name = chapterPostReq.Name;
            }

            newChapter.LastModifiedDate = DateTime.UtcNow;
            newChapter.LastModifiedBy = currentUserId;
            //newChapter.CreatorNote = chapterPostReq.CreatorNote;
            newChapter.IsEnableComment = chapterPostReq.IsEnableComment;
            newChapter.Permission = chapterPostReq.Permission;

            post.LastModifiedDate = DateTime.UtcNow;
            post.LastModifiedBy = currentUserId;

            await _postRepository.UpdateAsync(post);

            return newChapter;
        }
        public async Task<bool> DeleteChapter(string hashId, int order)
        {
            var subPost = await _postRepository
                .Connection.QueryFirstAsync<SubPost>(GetSubPostIdWithHashIdAndOrder, new { HashId = hashId, Order = order });

            var currentUserId = _currentUserService.Session.UserId;
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

                await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(subPost.PostId);
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
        public ChapterResponse MappingChapterResponse(SubPost newChapter)
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
            result.CreatedDate = newChapter.CreatedDate;
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

        public async Task<List<ChapterResponse>> SwapChapterOrder(string postHashId, ChapterOrderSwapReq orders)
        {
            var result = new List<ChapterResponse>();
            var subPosts = await _postRepository
                .Connection.QueryAsync<SubPost>(GetSubPostsWithHashIdAndOrders, new { HashId = postHashId, Order1 = orders?.Order1, Order2 = orders?.Order2 });

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
        private void VerifyPost(Post post, bool checkCompleted)
        {
            var currentUserId = _currentUserService.Session.UserId;
            if (post != null)
            {
                if (post.IsDelete)
                {
                    throw new BadRequestException(ApiErrorCode.POST_HAS_DELETED, ApiErrorMessage.POST_HAS_DELETED);
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
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
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

        private int GetOffsetSetup(ref TopPostReq loadReq)
        {
            var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

            if (loadReq.OrderBy == null)
            {
                loadReq.OrderBy = nameof(Post.CreatedDate);
            }
            return offset;
        }

        private void ValidateTotalItem(int number)
        {
            if (number > SystemConfig.PostItemCountMax)
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


        public async Task<List<RewardRespone>> CheckRewardsForPost(Guid userId, PostType type)
        {
            var rewards = new List<RewardRespone>();
            var check = await _postRepository
                .Connection.QueryFirstOrDefaultAsync<Guid?>(CheckUserFirstPost,
                new
                {
                    UserId = userId,
                    PostType = type
                });
            if (check == null)
            {
                RewardType rewardType = RewardType.FIRST_FEED;
                switch (type)
                {
                    case PostType.Story:
                        rewardType = RewardType.FIRST_STORY;
                        break;
                    case PostType.Comic:
                        rewardType = RewardType.FIRST_COMIC;
                        break;
                    default:
                        break;
                }

                await _userService.SyncWalletUserReward(userId, SystemConfig.DefaultRewardPoint, rewardType);
                rewards.Add(new RewardRespone()
                {
                    Type = rewardType,
                    MessageCode = rewardType.ToString()
                });
            }
            return rewards;

        }

        public async Task<IEnumerable<Guid>> GetPostRandomIdsAsync(GetPostRandomIdsReq req)
        {
            try
            {
                var numOfItem = 10;
                var numOfItemNeedFilter = numOfItem * 2;
                List<Guid> randomIds = new List<Guid>();

                if (req.NumOfItem != 0)
                {
                    numOfItem = req.NumOfItem;
                }

                if (req.PostRandomIds != null)
                {
                    numOfItemNeedFilter += req.PostRandomIds.Count;
                    randomIds = req.PostRandomIds;
                }

                var response = await _postRepository.Connection.QueryAsync<Guid>(GetPostRandomIdsQuery, new { numOfItemNeedFilter = numOfItemNeedFilter, numOfItem = numOfItem, postRandomIds = randomIds.ToList() });

                return response;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }
        #endregion

        #region REPORT
        public async Task<bool> ReportPostAsync(ReportPostReq req)
        {
            var postReport = new PostReport
            {
                PostId = req.PostId,
                UserId = _currentUserService.Session.UserId,
                ReasonType = req.ReasonType,
                ReasonText = req.ReasonText ?? ""
            };

            await _postReportValidator.OnValidate(postReport);

            var hasExisted = (await _postReportRepository.GetByCustomQuery(GetPostReportByPostIdAndUserId, new { postId = postReport.PostId, userId = postReport.UserId })).Any();

            if (hasExisted)
                return true;

            var iResult = await _postReportRepository.InsertAsync(postReport);
            return iResult > 0;
        }
        #endregion

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
