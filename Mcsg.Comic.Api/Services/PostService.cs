using AutoMapper;
using Dapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Comic.Api.Services;

using Analytic.Application.Protos;
using Common.Core;
using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Constants;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Lib.Common.Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Models.Earning;
using Requests;
using Validators;
using static Common.Core.Constants.Setting;
using static Common.Core.GoogleSheet;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class PostService : IPostService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="googleSheet">Sheets service</param>
    /// <param name="unitOfWork"></param>
    /// <param name="tagService"></param>
    /// <param name="smartLookupRepository"></param>
    /// <param name="fileService"></param>
    /// <param name="currentUserService"></param>
    /// <param name="mapper"></param>
    /// <param name="smartLookupService"></param>
    /// <param name="postReportValidator"></param>
    /// <param name="postCommentRepository"></param>
    public PostService(IMcsgContext context, ISetting setting, IStorageClient sc, GoogleSheet googleSheet, IUnitOfWork unitOfWork, ITagService tagService, IRepository<SmartLookup> smartLookupRepository, IFileService fileService, ICurrentUserService currentUserService, IMapper mapper, ISmartLookupService smartLookupService, IValidator<ComicPostReport> postReportValidator, IRepository<ComicPostComment> postCommentRepository)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _googleSheet = googleSheet;

        _unitOfWork = unitOfWork;
        _postRepository = unitOfWork.GetRepository<ComicPost>();
        _subPostRepository = unitOfWork.GetRepository<ComicSubPost>();
        _postReportRepository = unitOfWork.GetRepository<ComicPostReport>();
        _tagService = tagService;
        _smartLookupRepository = smartLookupRepository;
        _fileService = fileService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _smartLookupService = smartLookupService;
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
            throw new BadRequestException(nameof(E309), E309);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = DateTime.UtcNow, UserId = currentUserId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(postId, profileName);

            _ = Task.Run(async () => await SyncDeleteToAna(postId));

            return true;
        }
    }

    #region -- Post --
    public async Task<PostSeriesResponse> PostCreate(ComicPostCreateR request)
    {
        var vr = new ComicPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId.Value;
        var profileId = request.ProfileId;
        var profileName = request.ProfileName;

        //Check first post
        var rewards = await CheckRewardsForPost(userId, PostType.Comic);

        var thumbnailUrl = await GetPublicUrl(request.ThumbnailHashId);
        var coverUrl = await GetPublicUrl(request.CoverHashId);

        var hashId = PostConfig.HashLength.GetRandomString();
        var post = new ComicPost
        {
            Title = request.Title,
            Type = PostType.Comic,
            HashId = hashId,
            UserId = userId,
            AuthorId = request.IsCurrentUserAuthor ? userId : null,
            AuthorName = request.IsCurrentUserAuthor ? profileName : request.AuthorName,
            Body = request.Summary,
            ThumbnailUrl = thumbnailUrl,
            CoverUrl = coverUrl,
            IsMature = request.IsMature,
            Permission = request.Permission,
            Status = PostStatus.Public,
            CreatedBy = userId,
            Hide = HideOption.None,
            ViewCount = 0
        };

        var result = new NewPostSeriesResponse
        {
            Id = post.Id,
            Title = request.Title,
            HashId = hashId,
            UserId = userId,
            Type = post.Type,
            ThumbnailUrl = thumbnailUrl,
            CreatedOn = post.CreatedOn,
            Status = post.Status,
            Body = request.Summary,
            CoverUrl = coverUrl,
            IsMature = request.IsMature,
            Permission = request.Permission,
            ProfileId = profileId,
            AuthorName = post.AuthorName,
            IsCurrentUserAuthor = request.IsCurrentUserAuthor,
            Rewards = rewards
        };

        await _context.ComicPosts.AddAsync(post);
        await _context.SaveChangesAsync(default);

        await _smartLookupRepository.InsertAsync(new SmartLookup
        {
            CountCriteria = 0,
            Keyword = request.Title,
            KeywordType = LookupKeywordType.Comic
        });

        if (request.Tags != null && request.Tags.Count > 0)
        {
            result.Tags = (await _tagService.AddTagsToPost(post.Id, request.Tags, userId)).ToArray();
        }

        #region -- WriteDataToSheet --
        var dto = new PostSheetDto
        {
            Type = GoogleFileType.File1,
            SeriesType = PostType.Comic,
            Environment = _setting.Environment,
            Link = $"{_setting.Domain}/comic/series?id={post.HashId}",
            HashId = post.HashId,
            UserName = request.UserName,
            CreatedOn = post.CreatedOn,
            Title = post.Title,
            Platform = request.Platform
        };
        _ = Task.Run(async () => await _googleSheet.WriteDataToSheet(dto));
        #endregion

        _ = Task.Run(async () => await SyncCreateToAna(post));

        return result;
    }

    public async Task<PostSeriesResponse> GetSeries(ComicHashIdR req)
    {
        var isLoadChapters = req.IsLoadChapters;
        var hashId = req.HashId;
        var query = string.Format(GetSeriesQuery, _postRepository.TableName);
        string subNotLoadChapter = (isLoadChapters ? "" : @" AND sp.""Id"" IS NULL ");
        query = query.Replace("[Not-load-chapter]", subNotLoadChapter);

        var currentUserId = _currentUserService?.Session?.UserId;
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

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

                    if (subpost != null && subpost.Id != Guid.Empty)
                    {
                        subpost.ViewCount = subpost.ViewCount ?? 0;
                        subpost.IsCensored = !req.IsAdministrator && subpost.Status == PostStatus.Inactive && req.UserId != subpost.UserId;
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
                UserId = currentUserId,
                Hide = req.Hides,
                PostStatus = statusList,
                req.UserName
            }, splitOn: "Id, Id");
        //Add view
        if (dbPost == null)
        {
            throw new NotFoundException(E204, M204);
        }
        if ((dbPost.Status == PostStatus.Draft && dbPost.UserId != currentUserId))
        {
            throw new NotFoundException(E204, M204);
        }
        dbPost.TotalComment = await _postRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = hashId });
        dbPost.IsFollowing = currentUserId == null ? false : await _context.ComicPostFavoriteAvailable.AnyAsync(p => p.CreatedBy == currentUserId && p.PostId == dbPost.Id);
        var result = MappingFeedRespone(dbPost);

        result.CoverHashId = Path.GetFileNameWithoutExtension(result.CoverUrl);
        result.ThumbnailHashId = Path.GetFileNameWithoutExtension(result.ThumbnailUrl.RemoveNameSuffix());

        var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
        var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Comic.""ComicPostReactions"""), new
        {
            TargetIds = new List<Guid>() { dbPost.Id },
            UserId = currentUserId
        });

        if (postReactionResponse.Count() > 0)
        {
            MapReactionPostSeriesResponse(result, postReactionResponse.ToList());
        }

        result.FollowCount = await _context.ComicPostFavoriteAvailable.Where(p => p.PostId == result.Id).CountAsync();
        result.IsCensored = !req.IsAdministrator && result.Status == PostStatus.Inactive && req.UserName != result.UserName;
        result.IsBlur = result.Status == PostStatus.Inactive || result.IsMature;

        return result;
    }

    private void MapReactionPostSeriesResponse(PostSeriesResponse item, List<CommentReactionResponseQuery> reactions)
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

    public async Task<ChapterResponse> GetSeriesChapter(ChapterOrderR req)
    {
        var query = string.Format(GetSeriesChapterByHashIdWithJoinOrder, _postRepository.TableName);
        var userIsPremium = _currentUserService?.Session?.IsPremium ?? false;
        var currentUserId = _currentUserService?.Session?.UserId;
        var hashId = req.HashId;
        var order = req.Order;
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        ChapterResponse subpost = null;
        await _subPostRepository
            .Connection.QueryAsync<ChapterResponse, ComicResource, ChapterResponse>(query,
            (subpostdb, resource) =>
            {
                if (subpostdb == null)
                {
                    subpost = subpostdb;
                    subpost.Body = HttpUtility.HtmlDecode(subpostdb.Body);
                }
                if (subpostdb != null)
                {
                    if (subpost?.Id != subpostdb.Id)
                    {
                        subpost = subpostdb;
                        subpost.Body = HttpUtility.HtmlDecode(subpostdb.Body);
                    }

                    if (subpost != null && subpost.Files == null)
                    {
                        subpost.Files = new List<UploadFileDto>();
                    }

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
                UserId = currentUserId,
                Hide = req.Hides,
                PostStatus = statusList,
            }, splitOn: "Id, Id");

        if (subpost != null)
        {
            if (subpost.CreatedBy != currentUserId && subpost.UserId != currentUserId)
            {
                if (subpost.PublishDate != null && subpost.PublishDate > DateTime.UtcNow)
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
        }
        else
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, ApiErrorMessage.CHAPTER_NOT_EXIST);
        }

        subpost.IsCensored = !req.IsAdministrator && req.UserId != subpost.CreatedBy && subpost.Status == PostStatus.Inactive;
        if (subpost.IsCensored)
        {
            subpost.Files = [];
        }

        return subpost;
    }

    public async Task<PostSeriesAllTopResponse> GetTopSeries(PostType type)
    {
        var result = new PostSeriesAllTopResponse();
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        string allSubQuery = $@"({GetTopPostHitQuery})
                        UNION ALL
                        ({GetTopLatestPostHitQuery})
                        UNION ALL
                        ({GetTopLatestCompletePostHitQuery})";

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[AddNewUserNameContidion]", "")
            .Replace("[SelectPostIdsQuery]", allSubQuery)
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
                PostStatus = statusList
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
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

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

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[AddNewUserNameContidion]", "")
            .Replace("[SelectPostIdsQuery]", allSubQuery)
            .Replace("[CountResults]", countTopQuery)
            .Replace("[JoinSubPostSubQuery]", GetTopSubQueryJoinSubPostQuery)
            .Replace("[OrderBy]", "CreatedOn")
            .Replace("[Permission]", "");

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = type,
                    PageSize = request.PageSize,
                    Offet = offset,
                    LastWeek = (DateTime.UtcNow.AddDays(-7)),
                    PostStatus = statusList,
                    PostPermission = (int)PostPermission.Public,
                    TagName = request.HashTag,
                    UserId = currentUserId,
                    Hide = request.Hides
                });

        var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        var items = MapTopSeries(dbFeed.ToList());
        if (items != null && items.Count() > 0)
        {
            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Comic.""ComicPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                UserId = currentUserId
            });

            foreach (var item in items)
            {
                var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionPostSeiresTopResponse(item, postReaction);
                }
                item.IsCensored = request.UserName != item.UserName && !request.IsAdministrator && item.Status == PostStatus.Inactive;
                item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature == true;
            }

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
            var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.HashId == request.HashId);
            if (post == null)
            {
                throw new NotFoundException(E204, M204);
            }

            var offset = request.PageSize * (request.PageNumber - 1);
            var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

            string whereClause = " WHERE qpost1.\"Type\" = @PostType " +
                "AND qpost1.\"Permission\" = @PostPermission " +
                "AND qpost1.\"Status\" = ANY (@PostStatus) " +
                "AND qpost1.\"IsDelete\" = false AND qpost1.\"HashId\" != @HashId " +
                "AND (NOT (qpost1.\"Hide\" = ANY (@Hide) AND qpost1.\"Hide\" = ANY (@Hide) IS NOT NULL) OR qpost1.\"UserId\" = @UserId) ";
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
                        request.PageSize,
                        Offet = offset,
                        LastWeek = (DateTime.UtcNow.AddDays(-7)),
                        PostStatus = statusList,
                        TagIds = tagIds,
                        AuthorId = post.CreatedBy.Value,
                        request.HashId,
                        PostPermission = (int)PostPermission.Public,
                        Hide = request.Hides,
                        request.UserId
                    });

            var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var items = MapTopSeries(dbFeed.ToList());
            if (items != null && items.Count() > 0)
            {
                var results = new PagedResponse<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
                foreach (var item in items)
                {
                    item.IsCensored = !request.IsAdministrator && request.UserName != item.UserName && item.Status == PostStatus.Inactive;
                    item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature;
                }
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, ComicTopPostR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_TAG);
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    PostStatus = statusList,
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
        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_USER, profileName);
        var isMySelf = profileName == loadReq.UserName;
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    loadReq.PageSize,
                    Offet = offset,
                    PostStatus = statusList,
                    ProfileName = profileName,
                    Hide = loadReq.Hides,
                    MySelf = isMySelf
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);
            results.Items = MappingTopSeries(items);

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Comic.""ComicPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                UserId = _currentUserService?.Session?.UserId
            });

            foreach (var item in results.Items)
            {
                var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionPostSeiresTopResponse(item, postReaction);
                }
                item.IsCensored = !loadReq.IsAdministrator && loadReq.UserName != item.UserName && item.Status == PostStatus.Inactive;
                item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature == true;
            }
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
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = input.PageSize,
                    Offet = offset,
                    PostStatus = statusList,
                    TagName = input.TagName,
                    Hide = input.Hides
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
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var queryCondition = "";
        if (input.SearchBy == "ProfileName")
        {
            queryCondition = $@"WHERE u.""ProfileName""=@ProfileName 
                                   AND u.""IsDelete"" = false
                                   AND p.""Type""=@PostType
                                   AND p.""Status"" = ANY (@PostStatus)
                                   AND p.""Permission""=@Permission
                                   AND p.""IsDelete""=false
                                   AND NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" IS NOT NULL)";
        }
        else
        {
            queryCondition = $@" WHERE unaccent(p.""Title"") ILIKE unaccent('%{input.Keyword}%')
                                     AND p.""Type""=@PostType
                                     AND p.""Status"" = ANY (@PostStatus)
                                     AND p.""Permission""=@Permission
                                     AND p.""IsDelete""=false
                                     AND NOT (p.""Hide"" = ANY (@Hide) AND p.""Hide"" IS NOT NULL)";
        }

        var query = $@"SELECT p.""Id"",
                                  p.""Title"",
                                  p.""ThumbnailUrl"",
                                  p.""Body"",
                                  p.""IsMature"", 
                                  p.""HashId"",
                                  p.""Type"",
                                  p.""Hide"",
                                  p.""Status"",
                                  p.""ExternalResource"",
                                  GREATEST(p.""CreatedOn"", MAX(sp.""PublishDate"")) AS ""LatestCreatedOn"",
                                  u.""ProfileName"",
                                  u.""UserName"",
                                  CASE 
                                  WHEN COUNT(t.""Name"") > 0 THEN array_agg(DISTINCT t.""Name"") 
                                  ELSE NULL 
                                  END AS Tags,
                                  COUNT(pc.""Id"") as CommentCount,
                                  to_jsonb(array_agg(sp.*)) AS ""SubPostStr""
                                  FROM ""comic"".""ComicPosts""  p
                                  JOIN identity.""Users"" u ON  p.""CreatedBy""  = u.""Id"" 
                                  LEFT JOIN comic.""ComicTagPosts"" tp on p.""Id""  = tp.""PostId"" 
                                  LEFT JOIN ""Tags"" t on t.""Id""  = tp.""TagId"" 
                                  LEFT JOIN ""comic"".""ComicPostComments"" pc on pc.""PostId""  = p.""Id"" 
                                  LEFT JOIN LATERAL 
                                        (
                                            SELECT sp.""PostId"",sp.""Title"",sp.""Order"",sp.""CreatedOn"",sp.""PublishDate""
                                            FROM ""comic"".""ComicSubPosts"" sp 
                                            WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false AND sp.""PublishDate"" < @CurrentDate
                                            GROUP BY sp.""Id"", sp.""PostId"", sp.""Title"",sp.""Order""
                                            ORDER BY sp.""Order"" DESC
                                            LIMIT 2
                                        ) sp ON sp.""PostId"" = p.""Id""    
                                  [QueryCondition]
                                  GROUP BY p.""Id"" ,u.""ProfileName"", u.""UserName"", p.""Hide"", p.""Status"", p.""ExternalResource""
                                  ORDER BY p.""CreatedOn"" desc  
                                  OFFSET @Offset
                                  LIMIT @PageSize;

                                  SELECT COUNT(*) AS TotalCount
                                  FROM ""comic"".""ComicPosts""  p
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
                    PostStatus = statusList,
                    Permission = (int)PostPermission.Public,
                    ProfileName = input.Keyword,
                    Hide = input.Hides,
                    CurrentDate = DateTime.UtcNow
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostBoxResposne>(totalItems, input.PageNumber, input.PageSize);
            results.Items = MappingToPostBoxResponse(items);
            foreach (var item in results.Items)
            {
                item.IsCensored = input.UserName != item.UserName && !input.IsAdministrator && item.Status == PostStatus.Inactive;
                item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature == true;
                item.Chapters = item.Chapters.DistinctBy(p => p.Order).ToList();
                item.ChapterCount = item.Chapters.Count();
            }

            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"comic.""ComicPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                UserId = _currentUserService?.Session?.UserId
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
            results = new PagedResponse<PostBoxResposne>(0);
        }
        return results;
    }

    public async Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, ComicRecommendedR req)
    {
        var number = req.Number;
        ValidateTotalItem(number);
        var query = GetQuerySelectPage(PostSeriesSelectedType.RECOMMEND);
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var items = await _postRepository
                .Connection.QueryAsync<PostSeriesTopQueryDbResponse>(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = number,
                    PostStatus = statusList,
                    Hide = req.Hides
                });

        return MappingTopSeries(items);
    }

    public async Task<PostSeriesResponse> PostUpdate(ComicPostUpdateR request)
    {
        var vr = new ComicPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId.Value;
        var profileId = request.ProfileId;
        var profileName = request.ProfileName;

        var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.HashId == request.HashId);
        if (post == null)
        {
            throw new NotFoundException(E204, M204);
        }
        if (post.CreatedBy != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var thumbnailUrl = await GetPublicUrl(request.ThumbnailHashId);
        var coverUrl = await GetPublicUrl(request.CoverHashId);

        if (string.IsNullOrEmpty(request.Summary))
        {
            throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
        }

        var currentTitle = post.Title;
        post.Title = request.Title;
        post.HashId = request.HashId;
        post.AuthorId = request.IsCurrentUserAuthor ? userId : null;
        post.AuthorName = request.IsCurrentUserAuthor ? profileName : request.AuthorName;
        post.Body = request.Summary;
        post.ThumbnailUrl = thumbnailUrl.RemoveNameSuffix();
        post.CoverUrl = coverUrl;
        post.IsMature = request.IsMature;
        post.Permission = request.Permission;
        post.Status = request.IsSaveAndPublish ? post.Status : PostStatus.Draft;
        post.IsCompleted = request.IsCompleted;

        var result = new PostSeriesResponse
        {
            Id = post.Id,
            Title = request.Title,
            HashId = request.HashId,
            UserId = userId,
            Type = post.Type,
            ThumbnailUrl = post.ThumbnailUrl,
            CreatedOn = post.CreatedOn,
            Status = post.Status,
            Body = request.Summary,
            CoverUrl = coverUrl,
            IsMature = request.IsMature,
            Permission = request.Permission,
            ProfileId = profileId,
            AuthorName = post.AuthorName,
            IsCurrentUserAuthor = request.IsCurrentUserAuthor,
            IsCompleted = request.IsCompleted
        };

        await _context.SaveChangesAsync(default);

        if (currentTitle != request.Title)
        {
            var currentEntity = await _context.SmartLookupAvailable.FirstOrDefaultAsync(p => p.KeywordType == LookupKeywordType.Comic && p.Keyword == currentTitle);
            if (currentEntity != null)
            {
                currentEntity.Keyword = request.Title;
                await _smartLookupRepository.UpdateAsync(currentEntity);
            }

            _ = Task.Run(async () => await SyncUpdateToAna(post));
        }

        result.Tags = (await _tagService.UpdateTagsToPost(post.Id, request.Tags, userId)).ToArray();

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
            IsCurrentUserAuthor = currentUserId == item.UserId,
            ThumbnailUrl = item.ThumbnailUrl.AppendNameSuffix(),
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
            IsFollowing = item.IsFollowing,
            ExternalResource = item.ExternalResource,
            Hide = item.Hide
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

        if (!await _context.ComicPostAvailable.AnyAsync(p => p.Id == postId))
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var followedPost = await _context.ComicPostFavorites
                                                        .Where(p => p.CreatedBy == user.Id && p.PostId == postId)
                                                        .FirstOrDefaultAsync();
        if (followedPost == null)
        {
            await _context.ComicPostFavorites.AddAsync(new ComicPostFavorite
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
            _context.ComicPostFavorites.Update(followedPost);
            await _context.SaveChangesAsync(default);
            return !followedPost.IsDelete;
        }
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq)
    {

        ValidateTotalItem(loadReq.PageSize);
        var currentUserId = _currentUserService?.Session?.UserId;
        PagedResponse<PostSeriesTopResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(ComicPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostFollowedIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostFollowedCountQuery);

        var result = new PostSeriesAllTopResponse();

        var query = GetQuerySelectPage(PostSeriesSelectedType.FOLLOWED_POST);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    IsAccessPrivate = false,
                    UserId = currentUserId,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    Hide = loadReq.Hides,
                    PostStatus = statusList
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);

            var mappedItems = MappingTopSeries(items);
            var postIds = mappedItems.Select(x => x.Id).ToList();

            var followCounts = await _context.ComicPostFavoriteAvailable
                              .Where(p => postIds.Contains(p.PostId))
                              .GroupBy(p => p.PostId)
                              .Select(p => new { PostId = p.Key, Count = p.Count() })
                              .ToListAsync();

            foreach (var item in mappedItems)
            {
                item.FollowCount = followCounts.FirstOrDefault(p => p.PostId == item.Id)?.Count ?? 0;
            }
            results.Items = mappedItems;
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
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(ComicPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostCountQuery);

        var result = new PostSeriesAllTopResponse();
        var query = GetQuerySelectPage(PostSeriesSelectedType.BY_MYSELF, loadReq.UserName);


        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    UserId = currentUserId,
                    loadReq.PageSize,
                    Offet = offset,
                    Hide = loadReq.Hides,
                    PostStatus = statusList,
                    ProfileName = loadReq.UserName,
                    MySelf = true,
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);

            var mappedItems = MappingTopSeries(items);
            var postIds = mappedItems.Select(x => x.Id).ToList();

            var followCounts = await _context.ComicPostFavoriteAvailable
                              .Where(p => postIds.Contains(p.PostId))
                              .GroupBy(p => p.PostId)
                              .Select(p => new { PostId = p.Key, Count = p.Count() })
                              .ToListAsync();

            foreach (var item in mappedItems)
            {
                item.FollowCount = followCounts.FirstOrDefault(p => p.PostId == item.Id)?.Count ?? 0;
            }
            results.Items = mappedItems;
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
                    from ""comic"".""ComicPostComments"" pc 
                    left join ""comic"".""ComicPosts""  p on  pc.""PostId"" = p.""Id""
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
                    from ""comic"".""ComicSubPostComments"" spc 
                    left join ""comic"".""ComicSubPosts"" sp on  spc.""PostId"" = sp.""Id""
                    LEFT JOIN ""comic"".""ComicPosts""  p on p.""Id"" = sp.""PostId""
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
                        FROM ""comic"".""ComicPostComments"" pc 
                        LEFT JOIN ""comic"".""ComicPosts""  p on  pc.""PostId"" = p.""Id""
                        LEFT JOIN ""comic"".""ComicPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
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
                        FROM ""comic"".""ComicSubPostComments"" spc 
                        LEFT join ""comic"".""ComicSubPosts"" sp on  spc.""PostId"" = sp.""Id""
                        LEFT JOIN ""comic"".""ComicSubPostCommentReactions""  spcr ON spc.""Id"" = spcr.""TargetId""
                        LEFT JOIN ""comic"".""ComicPosts""  p on p.""Id"" = sp.""PostId""
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

    public async Task<List<PostBoxResponse>> GetPostDetails(PaginatedR req)
    {
        var hashIds = req.HashIds;
        var statusList = new List<int> { (int)PostStatus.Inactive, (int)PostStatus.Public };

        var param = new
        {
            HashIds = hashIds.Split(',').ToList(),
            Hide = req.Hides,
            PostStatus = statusList,
            CurrentDate = DateTime.UtcNow
        };
        var result = await _postRepository.Connection.QueryAsync<PostBoxQueryResponse>(GetPostDetailsQuery, param);
        var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;

        if (result != null && result.Any())
        {
            var listPostDetails = new List<PostBoxResponse>();

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Comic.""ComicPostReactions"""), new
            {
                TargetIds = result.Select(p => p.Id).ToList(),
                UserId = currentUserId
            });

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
                    ChapterCount = chapters?.Count ?? 0,
                    Type = res.Type,
                    HashId = res.HashId,
                    CreatedOn = res.CreatedOn,
                    ProfileName = res.ProfileName,
                    UserId = res.UserId,
                    UserName = res.UserName,
                    LatestCreatedOn = res.LatestCreatedOn,
                    Hide = res.Hide,
                    Status = res.Status,
                    IsExternalSource = res.IsExternalSource,
                    IsCensored = !req.IsAdministrator && req.UserName != res.UserName && res.Status == PostStatus.Inactive,
                    IsBlur = res.Status == PostStatus.Inactive || res.IsMature == true,
                };

                var postReaction = postReactionResponse.Where(p => p.TargetId == res.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionResponse(postDetails, postReaction);
                }

                listPostDetails.Add(postDetails);
            }

            return listPostDetails;
        }
        else
        {
            return new List<PostBoxResponse>();
        }
    }

    private void MapPostBoxReactionResponse(PostBoxResposne item, List<CommentReactionResponseQuery> reactions)
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

    private void MapReactionResponse(PostBoxResponse item, List<CommentReactionResponseQuery> reactions)
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

    private void MapReactionPostSeiresTopResponse(PostSeriesTopResponse item, List<CommentReactionResponseQuery> reactions)
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

    public UploadFileDto MappingFile(ComicResource resources)
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
            Url = _sc.GetCdnUrl(resources.Url, resources.BucketName, resources.MinioInstance, resources.Type),
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
            TotalComment = x.TotalComment,
            //"AuthorName", "CoverUrl","CreatedOn", "IsMature", "Id", "Permission", "Status", "UserId"
            HashId = x.HashId,
            Chapters = MappingTopChapter(x.SubPostStr),
            Hide = x.Hide,
            ExternalResource = x.ExternalResource,
            LatestCreatedOn = x.LatestCreatedOn
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
            Hide = x.Hide,
            ExternalResource = x.ExternalResource,
            Status = x.Status,
            LatestCreatedOn = x.LatestCreatedOn
        }).ToList();
    }

    private List<PostSeriesTopResponse> MapTopSeries(IEnumerable<PostSeriesTopQueryDbResponse> posts)
    {
        var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;

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
            IsCurrentUserAuthor = currentUserId == x.UserId,
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
            Hide = x.Hide,
            Reaction = new ReactionsResponse
            {
                TotalReacts = x.TotalReact,
                Reactions = x.ReactionByPostStr != null ? JsonConvert.DeserializeObject<List<ReactionResponse>>(x.ReactionByPostStr) : new List<ReactionResponse>()
            },
            LatestCreatedOn = x.LatestCreatedOn,
            ExternalResource = x.ExternalResource
        }).ToList();
    }

    private string GetQuerySelectPage(PostSeriesSelectedType selectedType, string? userName = "")
    {
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;
        var permission = $@"AND p.""Permission"" != 1";
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
            case PostSeriesSelectedType.BY_MYSELF:
                {
                    permission = "";
                    topSelectPostIdQuery = GetMyPostIdsQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostCountQuery);
                    break;
                }
            case PostSeriesSelectedType.FOLLOWED_POST:
                {
                    topSelectPostIdQuery = GetMyPostFollowedIdsQuery; ;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostFollowedCountQuery);
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
            .Replace("[OrderBy]", "CreatedOn")
            .Replace("[Permission]", permission);

        if (userName != null && userName != "")
        {
            var newUserNameQuery = $@"OR (u.""UserName"" = @ProfileName AND @MySelf)";
            query = query.Replace("[AddNewUserNameContidion]", newUserNameQuery);
        }
        else
        {
            query = query.Replace("[AddNewUserNameContidion]", "");
        }

        return query;
    }
    #endregion

    #region -- Subpost --
    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, ComicChapterListR loadReq)
    {
        PagedResponse<ChapterResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(ComicSubPost.Sort);
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
                item.Body = HttpUtility.HtmlDecode(item.Body);
            }
            results.Items = items;
        }
        else
        {
            results = new PagedResponse<ChapterResponse>(0);
        }
        return results;
    }

    public async Task<List<ChapterList>> GetAllChapters(string hashId)
    {
        var postId = await _context.ComicPostAvailable.Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        if (postId == Guid.Empty)
        {
            throw new NotFoundException(E204, M204);
        }
        return await _context.ComicSubPostAvailable.Where(p => p.PostId == postId)
            .OrderBy(p => p.Sort)
            .Select(p => new ChapterList
            {
                Sort = p.Sort,
                Order = p.Order,
                Title = p.Title
            }).ToListAsync();
    }

    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(string hashId)
    {
        PagedResponse<ChapterTOCResponse> results;
        var query = GetSeriesChaptersSimpleByHashId;

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostHashId = hashId,
                    CurrentDate = DateTime.UtcNow
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
            loadReq.OrderBy = nameof(ComicSubPost.Order);
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

    public async Task<ChapterResponse> SubPostCreate(ComicSubPostCreateR request)
    {
        var vr = new ComicSubPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        if (!request.IsPublicNow && request.PublishDate == null)
        {
            throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
        }

        var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.HashId == request.PostHashId);
        if (post == null)
        {
            throw new NotFoundException(E204, M204);
        }

        if (post.IsCompleted == true)
        {
            throw new ForbiddenAccessException(ApiErrorCode.POST_HAS_COMPLETED, ApiErrorMessage.POST_HAS_COMPLETED);
        }

        var hasSubPost = await _context.ComicSubPostAvailable.AnyAsync(p => p.PostId == post.Id && p.Order == request.Order);
        if (hasSubPost)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }

        var newOrder = await DetermineOrder(post.Id, request.Order, request.IsAutoGenerateOrder);

        var userId = request.UserId.Value;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var userName = request.UserName;

        //Check first post
        var rewards = await CheckRewardsForSubPost(userId);

        var subPost = new ComicSubPost
        {
            AuthorId = post.AuthorId,
            CreatedBy = userId,
            Permission = request.Permission,
            Order = newOrder,
            Name = string.IsNullOrEmpty(request.Name) ? newOrder.ToString() : request.Name,
            PostId = post.Id,
            Status = PostStatus.Public,
            PublishDate = request.IsPublicNow ? DateTime.UtcNow : request.PublishDateUtc,
            Title = request.Title,
            UserId = userId,
            //CreatorNote = request.CreatorNote,
            IsEnableComment = request.IsEnableComment,
            ViewCount = 0,
            HashId = PostConfig.SubHashLength.GetRandomString(),
            IsExclusive = false, //BCW-37
            IsPremium = request.IsPremium,
            PostHashId = post.HashId,
            Sort = !await _context.ComicSubPostAvailable.AnyAsync(p => p.PostId == post.Id) ? 1 :
                    await _context.ComicSubPostAvailable.Where(p => p.PostId == post.Id).MaxAsync(p => p.Sort) + 1
        };

        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = userId;

        await _context.ComicSubPosts.AddAsync(subPost);
        await _context.SaveChangesAsync(default);

        var result = MappingChapterResponse(subPost);
        result.Rewards = rewards;
        if (request?.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, subPost.PostId, subPost.PostHashId)
            {
                SubPostId = subPost.Id,
                Order = subPost.Order
            };
            result.Files = await _fileService.ProcessComicFilesAsync(urDto);
        }

        #region -- WriteDataToSheet --
        var dto = new SubPostSheetDto
        {
            SeriesName = post.Title,
            Type = GoogleFileType.File2,
            SeriesType = PostType.Comic,
            Environment = _setting.Environment,
            Link = $"{_setting.Domain}/comic/view-chapter?comicid={post.HashId}&order={subPost.Order}",
            HashId = subPost.HashId,
            UserName = request.UserName,
            CreatedOn = subPost.CreatedOn,
            Title = subPost.Title,
            Platform = request.Platform
        };
        _ = Task.Run(async () => await _googleSheet.WriteDataToSheet(dto));
        #endregion

        await CreateSubPost(subPost);

        return result;
    }

    public async Task<ChapterResponse> SubPostUpdate(ComicSubPostUpdateR request)
    {
        var vr = new ComicSubPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        if (!request.IsPublicNow && request.PublishDate == null)
        {
            throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
        }

        var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.HashId == request.PostHashId);
        if (post == null)
        {
            throw new NotFoundException(E204, M204);
        }

        var subPost = await FindSubPost(request.PostHashId, request.ChapterOrder);
        if (subPost == null)
        {
            throw new NotFoundException(nameof(E208), E208);
        }

        var hasSubPost = await _context.ComicSubPostAvailable.AnyAsync(p => p.PostId == post.Id && p.Order == request.Order && p.Id != subPost.Id);
        if (hasSubPost)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }

        subPost.PublishDate = request.IsPublicNow ? DateTime.UtcNow : request.PublishDateUtc;
        subPost.Title = request.Title;
        if (!string.IsNullOrEmpty(request.Name))
        {
            subPost.Name = request.Name;
        }

        var userId = request.UserId.Value;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var userName = request.UserName;

        subPost.ModifiedOn = DateTime.UtcNow;
        subPost.ModifiedBy = userId;
        //subPost.CreatorNote = request.CreatorNote;
        subPost.IsEnableComment = request.IsEnableComment;
        subPost.Permission = request.Permission;
        subPost.IsPremium = request.IsPremium;
        subPost.PostHashId = request.PostHashId;
        subPost.Order = request.Order ?? request.ChapterOrder;
        subPost.Sort = subPost.Sort;
        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = userId;

        await _context.SaveChangesAsync(default);

        var result = MappingChapterResponse(subPost);
        if (request?.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, subPost.PostId, subPost.PostHashId)
            {
                SubPostId = subPost.Id,
                Order = subPost.Order
            };
            result.Files = await _fileService.ProcessComicFilesAsync(urDto);
        }

        return result;
    }

    public async Task<bool> DeleteChapter(string hashId, float order)
    {
        var ss = _currentUserService.Session;
        var currentUserId = ss.UserId;
        var profileName = ss.ProfileName;

        var subPost = await FindSubPost(hashId, order);
        if (subPost == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, order));
        }
        else if (subPost.UserId != currentUserId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeleteSubPost, new { SubPostId = subPost.Id, Date = DateTime.UtcNow, UserId = currentUserId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(subPost.PostId, profileName);

            await DeleteSubPost(subPost.Id);

            return true;
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

    public ChapterResponse MappingChapterResponse(ComicSubPost newChapter)
    {
        var result = new ChapterResponse();

        result.Id = newChapter.Id;
        result.HashId = newChapter.HashId;
        result.Body = HttpUtility.HtmlDecode(newChapter.Body);
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
        result.IsEnableComment = newChapter.IsEnableComment;
        result.IsExclusive = newChapter.IsExclusive;
        //When new, return 0
        result.CommentCount = 0;
        return result;
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        var result = new List<ChapterResponse>();
        var currentUserId = _currentUserService.Session.UserId;
        var postId = await _context.ComicPostAvailable.Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        var fromOrder = orders.Order1;
        var toOrder = orders.Order2;
        var chapterFr = await _context.ComicSubPostAvailable.Where(x => x.Sort == orders.Order1 && x.PostId == postId).FirstOrDefaultAsync();
        var chapterTo = await _context.ComicSubPostAvailable.Where(x => x.Sort == orders.Order2 && x.PostId == postId).FirstOrDefaultAsync();
        if (chapterFr == null || chapterTo == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, chapterFr == null ? orders?.Order1 : orders?.Order2));
        }

        // Check owner
        if (chapterFr.UserId != currentUserId || chapterTo.UserId != currentUserId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }

        if (fromOrder < toOrder)
        {
            await _context.ComicSubPostAvailable.Where(c => c.Sort > fromOrder && c.Sort <= toOrder && c.PostId == postId)
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort - 1));
        }
        else
        {
            await _context.ComicSubPostAvailable.Where(c => c.Sort < fromOrder && c.Sort >= toOrder && c.PostId == postId)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort + 1));
        }

        chapterFr.Sort = orders.Order2;

        await _context.SaveChangesAsync(default);
        return result;
    }

    public async Task MoveChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        var currentUserId = _currentUserService.Session.UserId;
        var postId = await _context.ComicPostAvailable.Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        var fromOrder = orders.Order1;
        var toOrder = orders.Order2;
        var chapterFr = await _context.ComicSubPostAvailable.Where(x => x.Sort == orders.Order1 && x.PostId == postId).FirstOrDefaultAsync();
        var chapterTo = await _context.ComicSubPostAvailable.Where(x => x.Sort == orders.Order2 && x.PostId == postId).FirstOrDefaultAsync();
        if (chapterFr == null || chapterTo == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, chapterFr == null ? orders?.Order1 : orders?.Order2));
        }

        // Check owner
        if (chapterFr.UserId != currentUserId || chapterTo.UserId != currentUserId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }

        if (fromOrder < toOrder)
        {
            await _context.ComicSubPostAvailable.Where(c => c.Sort > fromOrder && c.Sort < toOrder && c.PostId == postId)
           .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort - 1));
        }
        else
        {
            await _context.ComicSubPostAvailable.Where(c => c.Sort >= toOrder && c.Sort < fromOrder && c.PostId == postId)
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort + 1));
        }

        chapterFr.Sort = fromOrder < toOrder ? toOrder - 1 : toOrder;

        await _context.SaveChangesAsync(default);
    }
    #endregion

    private int GetOffsetSetup(ref ComicTopPostR loadReq)
    {
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(ComicPost.CreatedOn);
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

        var check = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
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
                               FROM ""comic"".""ComicSubPosts"" sp
                               JOIN ""comic"".""ComicPosts""  p ON sp.""PostId"" = p.""Id""
                               JOIN ""comic"".""ComicResources"" r on sp.""Id"" = r.""SubPostId""
                               WHERE p.""IsDelete"" = false
                               AND sp.""IsDelete"" = false
                               [QueryByType]
                               [IgnoreQuery]
                               AND sp.""Order"" = (
                                                    SELECT MIN(sp_inner.""Order"")
                                                    FROM ""comic"".""ComicSubPosts"" sp_inner
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
            var query = @$"SELECT ""HashId"" From ""comic"".""ComicPosts""  
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

    /// <summary>
    /// GetPublicUrl
    /// </summary>
    /// <param name="hashId"></param>
    /// <returns></returns>
    private async Task<string> GetPublicUrl(string hashId)
    {
        var resource = await _context.ComicResources.Where(p => p.HashId == hashId)
            .Select(p => new { p.Url, p.BucketName, p.MinioInstance })
            .FirstOrDefaultAsync();
        if (resource == null)
        {
            return string.Empty;
        }
        var minioInstance = resource.MinioInstance ?? MinioInstanceType.Default;
        return _setting.GetMinio(minioInstance).GetPublicUrl(resource.BucketName, resource.Url);
    }

    /// <summary>
    /// Determines the order for a comic subpost.
    /// </summary>
    /// <param name="postId">The ID of the parent post.</param>
    /// <param name="order">The provided order value, if any.</param>
    /// <param name="isAutoGenerateOrder">Flag indicating whether the order should be auto-generated.</param>
    /// <returns>The calculated order as a float.</returns>
    private async Task<float> DetermineOrder(Guid postId, float? order, bool isAutoGenerateOrder)
    {
        if (!isAutoGenerateOrder)
        {
            return order >= 0 ? order.Value : 0;
        }

        var orders = await _context.ComicSubPostAvailable.Where(p => p.PostId == postId).Select(p => p.Order).ToListAsync();

        return orders.Count > 0 ? (int)orders.Max() + 1 : 1;
    }

    /// <summary>
    /// Finds a specific subpost based on the parent post's hash ID and the subpost's order.
    /// </summary>
    /// <param name="postHashId">The hash ID of the parent post.</param>
    /// <param name="order">The order of the subpost.</param>
    /// <returns>The matching subpost if found; otherwise, null.</returns>
    private async Task<ComicSubPost?> FindSubPost(string? postHashId, float order)
    {
        var q = from a in _context.ComicSubPostAvailable
                join b in _context.ComicPostAvailable on a.PostId equals b.Id
                where b.HashId == postHashId && a.Order == order
                select a;
        return await q.FirstOrDefaultAsync();
    }

    public async Task<List<RewardDto>> CheckRewardsForSubPost(Guid userId)
    {
        var res = new List<RewardDto>();

        var check = await _context.ComicSubPosts.FirstOrDefaultAsync(p => p.UserId == userId);
        if (check == null)
        {
            var rewardType = RewardType.FirstComic;
            res.Add(new RewardDto
            {
                Type = rewardType,
                MessageCode = rewardType.ToString()
            });
        }

        return res;
    }

    #region -- Post --
    private async Task<ComicCreateRsp> SyncCreateToAna(ComicPost ett)
    {
        var res = new ComicCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new ComicProto.ComicProtoClient(channel);
            var request = new ComicCreateReq
            {
                Items =
                {
                    new ComicProtoDto
                    {
                        PostId = ett.Id.ToString(),
                        HashId = ett.HashId,
                        UserId = ett.UserId.ToString(),
                        Title = ett.Title,
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy.ToString()
                    }
                }
            };

            var rsp = await client.CreateAsync(request);
            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<ComicUpdateRsp> SyncUpdateToAna(ComicPost ett)
    {
        var res = new ComicUpdateRsp() { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new ComicProto.ComicProtoClient(channel);
            var request = new ComicUpdateReq
            {
                PostId = ett.Id.ToString(),
                Title = ett.Title,
            };

            var rsp = await client.UpdateAsync(request);
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

    private async Task<ComicDeleteRsp> SyncDeleteToAna(Guid id)
    {
        var res = new ComicDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new ComicProto.ComicProtoClient(channel);
            var request = new ComicDeleteReq
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

    #region -- SubPost --
    private async Task<ComicSubCreateRsp> CreateSubPost(ComicSubPost ett)
    {
        var res = new ComicSubCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new ComicSubProto.ComicSubProtoClient(channel);
            var request = new ComicSubCreateReq
            {
                Items =
                {
                    new ComicSubProtoDto
                    {
                        PostId = ett.PostId.ToString(),
                        SubPostId = ett.Id.ToString(),
                        UserId = ett.UserId.ToString(),
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy.ToString()
                    }
                }
            };

            var rsp = await client.CreateAsync(request);
            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<ComicSubDeleteRsp> DeleteSubPost(Guid id)
    {
        var res = new ComicSubDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new ComicSubProto.ComicSubProtoClient(channel);
            var request = new ComicSubDeleteReq
            {
                SubPostId = id.ToString()
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
    /// Google sheet
    /// </summary>
    private readonly GoogleSheet _googleSheet;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<ComicPost> _postRepository;
    private readonly IRepository<ComicPostComment> _postCommentRepository;
    private readonly IRepository<ComicSubPost> _subPostRepository;
    private readonly IRepository<ComicPostReport> _postReportRepository;
    private readonly ITagService _tagService;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IFileService _fileService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ISmartLookupService _smartLookupService;

    #endregion
}
