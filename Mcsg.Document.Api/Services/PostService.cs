using AutoMapper;
using Dapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Document.Api.Services;

using Analytic.Application.Protos;
using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Constants;
using Dtos;
using Extensions;
using Interfaces;
using Models;
using Models.Earning;
using Requests;
using Validators;
using static Common.Core.Constants.Setting;
using static Common.Core.Extensions.StringExtension;
using static Common.SeedWork.Constants.Error;

public partial class PostService : BaseMinioS, IPostService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="businessText"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="tagService"></param>
    /// <param name="fileService"></param>
    /// <param name="mapper"></param>
    /// <param name="smartLookupService"></param>
    /// <param name="postCommentRepository"></param>
    public PostService(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessText, IUnitOfWork unitOfWork, ITagService tagService, IFileService fileService, IMapper mapper, ISmartLookupService smartLookupService, IRepository<DocumentPostComment> postCommentRepository) : base(context, setting, sc)
    {
        _businessText = businessText;
        _postRepository = unitOfWork.GetRepository<DocumentPost>();
        _subPostRepository = unitOfWork.GetRepository<DocumentSubPost>();
        _subPostCommentRepository = unitOfWork.GetRepository<SocialSubPostComment>();
        _tagService = tagService;
        _fileService = fileService;
        _mapper = mapper;
        _smartLookupService = smartLookupService;
        _postCommentRepository = postCommentRepository;
    }

    public async Task<bool> Delete(IdBaseR request)
    {
        var postId = request.Id;
        var userId = request.UserId;
        var profileName = request.ProfileName;

        var feedDb = await _postRepository.GetByIdAsync(postId);
        if (feedDb == null)
        {
            throw new BadRequestException(nameof(E204), E204);
        }
        else if (feedDb.UserId != userId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = DateTime.UtcNow, UserId = userId });

            // Delete ThumbnailUrl and CoverUrl
            await HandleThumbnailUrlAndCoverUrl(feedDb, true, userId.Value);

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(postId, profileName);

            _ = Task.Run(async () => await SyncDeleteToAna(postId));

            return true;
        }
    }

    #region -- Post --
    public async Task<PostSeriesResponse> PostCreate(DocumentPostCreateR request)
    {
        var vr = new DocumentPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(nameof(E000), t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109));
        }

        var userId = request.UserId.Value;
        var profileId = request.ProfileId;
        var profileName = request.ProfileName;

        //Check first post
        var rewards = await CheckRewardsForPost(userId, PostType.Document);

        var thumbnailUrl = await GetPublicUrl(request.ThumbnailHashId);
        var coverUrl = await GetPublicUrl(request.CoverHashId);

        var hashId = PostConfig.HashLength.GetRandomString();
        var post = new DocumentPost
        {
            Title = request.Title,
            Type = PostType.Document,
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
            IsAllowDownload = request.IsAllowDownload,
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

        await _context.DocumentPosts.AddAsync(post);

        var smartLookup = new SmartLookup
        {
            CountCriteria = 0,
            Keyword = request.Title,
            KeywordType = LookupKeywordType.Document
        };
        await _context.SmartLookups.AddAsync(smartLookup);

        await _context.SaveChangesAsync(default);

        // Update field IsDelete of ThumbnailUrl and CoverUrl
        await HandleThumbnailUrlAndCoverUrl(post, false, userId);

        if (request.Tags != null && request.Tags.Count > 0)
        {
            result.Tags = (await _tagService.AddTagsToPost(post.Id, request.Tags, userId)).ToArray();
        }

        _ = Task.Run(async () => await SyncCreateToAna(post));

        return result;
    }

    public async Task<PostSeriesTopResponse> GetSeries(DocumentHashIdR req)
    {
        PostSeriesTopResponse res;

        var hashId = req.HashId;
        var userId = req.UserId;

        var pPost = new
        {
            HashId = hashId,
            req.Hides,
            PostStatuses = StatusUtils.PostStatusInt
        };
        var qPost = @"SELECT * FROM document.fw_document_post(@HashId, @Hides, @PostStatuses)";

        using (var connection = _context.Database.GetDbConnection())
        {
            res = (await connection.QueryAsync<PostSeriesTopResponse>(qPost, pPost)).FirstOrDefault() ?? new PostSeriesTopResponse();
            if (res == null || (res.Status == PostStatus.Draft && res.UserId != userId))
            {
                throw new NotFoundException(nameof(E204), E204);
            }

            #region -- Reaction --
            var qReaction = @"SELECT * FROM document.fw_document_reaction_counts(@TargetIds, @UserId)";
            var pReaction = new { TargetIds = new List<Guid> { res.Id }, UserId = userId };
            var reactions = await connection.QueryAsync<CommentReactionResponseQuery>(qReaction, pReaction);
            #endregion

            #region -- Subposts --
            var pSubPost = new
            {
                PostId = res.Id,
                UserId = userId,
                PostStatuses = StatusUtils.PostStatusInt,
                req.IsLoadChapters
            };
            var qSubPost = @"SELECT * FROM document.fw_document_subposts(@PostId, @UserId, @PostStatuses, @IsLoadChapters)";
            var subposts = await connection.QueryAsync<ChapterBasicResponse>(qSubPost, pSubPost);
            #endregion

            var latestModifiedOn = res.ModifiedOn;
            res.Chapters = subposts.Where(p => p?.Id != Guid.Empty)
                .Select(p =>
                {
                    p.IsCensored = !req.IsAdministrator && p.Status == PostStatus.Inactive && req.UserId != p.UserId;
                    latestModifiedOn = p.ModifiedOn > latestModifiedOn ? p.ModifiedOn : latestModifiedOn;
                    return p;
                })
                .ToList();

            res.TotalComment = await _context.Available<DocumentPost>().Where(p => p.HashId == hashId)
                .Select(p => new
                {
                    PostComments = p.DocumentPostComments.Count(q => !q.IsDelete),
                    SubPostComments = p.DocumentSubPosts.Sum(q => q.DocumentSubPostComments.Count(x => !x.IsDelete))
                })
                .Select(p => p.PostComments + p.SubPostComments)
                .SumAsync();
            res.IsFollowing = userId != null && await _context.Available<DocumentPostFavorite>().AnyAsync(p => p.CreatedBy == userId && p.PostId == res.Id);

            MappingFeedRespone(res);

            res.ModifiedOn = latestModifiedOn;
            res.CoverHashId = Path.GetFileNameWithoutExtension(res.CoverUrl);
            res.ThumbnailHashId = Path.GetFileNameWithoutExtension(res.ThumbnailUrl.RemoveNameSuffix());

            if (reactions.Any())
            {
                MapReactionPostSeriesResponse(res, reactions.ToList());
            }

            res.LatestCreatedOn = res.Chapters?.Max(p => p.PublishDate) ?? res.CreatedOn;
            res.FollowCount = await _context.Available<DocumentPostFavorite>().CountAsync(p => p.PostId == res.Id);
            res.IsCensored = !req.IsAdministrator && res.Status == PostStatus.Inactive && req.UserName != res.UserName;
            res.IsBlur = res.Status == PostStatus.Inactive || res.IsMature;
            res.IsCurrentUserAuthor = req.UserId == res.UserId;
        }

        return res;
    }

    private void MapReactionPostSeriesResponse(PostSeriesTopResponse item, List<CommentReactionResponseQuery> reactions)
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
        var isPremium = req.IsPremium;
        var userId = req.UserId;
        var hashId = req.HashId;
        var order = req.Order;
        var isAdministrator = req.IsAdministrator;

        var query = string.Format(GetSeriesChapterByHashIdWithJoinOrder, _postRepository.TableName);

        ChapterResponse subpost = null;
        await _subPostRepository
            .Connection.QueryAsync<ChapterResponse, DocumentResource, ChapterResponse>(query,
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
                UserId = userId,
                Hide = req.Hides,
                PostStatus = StatusUtils.PostStatusInt,
                PostStatusForAuthor = StatusUtils.PostStatusForAuthorInt
            }, splitOn: "Id, Id");

        if (subpost == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, ApiErrorMessage.CHAPTER_NOT_EXIST);
        }

        //Validate premium
        if (subpost.IsPremium)
        {
            if (userId == null)
            {
                subpost.Files = [];
                subpost.Body = "";
                subpost.IsAllowDownload = false;
            }

            var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == userId);
            if (subpost.UserId != user?.Id && user?.IsPremium != true && !isAdministrator)
            {
                subpost.Files = [];
                subpost.Body = "";
                subpost.IsAllowDownload = false;
            }
        }

        if (subpost.CreatedBy != userId && subpost.UserId != userId)
        {
            if (subpost.PublishDate != null && subpost.PublishDate > DateTime.UtcNow)
            {
                throw new BadRequestException(nameof(E204), E204);
            }
            //Check permission
            if (subpost.Permission == PostPermission.Private)
            {
                throw new BadRequestException(nameof(E204), E204);
            }
            //Check IsExclusive
            if (subpost.IsExclusive && subpost.UserExclusiveId == null)
            {
                throw new BadRequestException(ApiErrorCode.NEED_BUY_TO_READ, ApiErrorMessage.NEED_BUY_TO_READ);
            }
            if (subpost.Permission == PostPermission.Premium && (!isPremium && subpost.UserExclusiveId == null))
            {
                //Todo implement Premium
                throw new BadRequestException(ApiErrorCode.NEED_PREMIUM_TO_READ, ApiErrorMessage.NEED_PREMIUM_TO_READ);
            }
        }

        subpost.IsCensored = !isAdministrator && userId != subpost.CreatedBy && subpost.Status == PostStatus.Inactive;
        if (subpost.IsCensored)
        {
            subpost.Files = [];
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
                PostStatus = StatusUtils.PostStatusInt,
                PostPermission = (int)PostPermission.Public,
            });
        result = new PostSeriesAllTopResponse();// MappingTopSeries(dbFeed);
        var listHit = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.Hit).ToList();
        var listLatest = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.Latest).ToList();
        var listLatestCompleted = dbFeed.Where(x => x.SelectType == PostSeriesSelectedType.Completed).ToList();
        result.TopHits = MappingTopSeries(listHit);
        result.TopLatest = MappingTopSeries(listLatest);
        result.TopCompleted = MappingTopSeries(listLatestCompleted);

        return result;
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesAsync(PostType type, DocumentPostListSeriesR request)
    {
        var userId = request.UserId;
        var isFavorite = userId == null ? false : request.IsFavorite;
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

        var query = GetTopAllPostAllTypeByTagQuery.Replace("[AddNewUserNameContidion]", "")
            .Replace("[SelectPostIdsQuery]", allSubQuery)
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
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public,
                    TagName = request.HashTag,
                    UserId = userId,
                    Hide = request.Hides
                });

        var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        var items = MapTopSeries(dbFeed.ToList(), request.UserId);
        if (items != null && items.Count() > 0)
        {
            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Document.""DocumentPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                UserId = userId
            });

            var commentsResult = await GetMostCommentReaction(items.Select(p => p.HashId).ToList());
            var comments = await GetReactionAndMentionOfComment(commentsResult.Comments, userId);

            foreach (var item in items)
            {
                var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postReaction.Count > 0)
                {
                    MapReactionPostSeiresTopResponse(item, postReaction);
                }

                item.IsCensored = request.UserName != item.UserName && !request.IsAdministrator && item.Status == PostStatus.Inactive;
                item.IsBlur = item.Status == PostStatus.Inactive || item.IsMature == true;

                var totalComments = commentsResult.TotalComment?.FirstOrDefault(p => p.PostHashId == item.HashId)?.TotalCommentCount;
                item.Comments = new CommentPagedResults<MostReactionCommentResponse>(totalComments ?? 0, 1, 2)
                {
                    Items = comments.Where(p => p.PostHashId == item.HashId),
                    TotalComments = totalComments ?? 0
                };
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetRelationSeriesAsync(PostType type, DocumentRelationPostSeriesR request)
    {
        try
        {
            var post = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.HashId == request.HashId);
            if (post == null)
            {
                throw new NotFoundException(nameof(E204), E204);
            }

            var offset = request.PageSize * (request.PageNumber - 1);

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
                        PostStatus = StatusUtils.PostStatusIntPublic,
                        TagIds = tagIds,
                        AuthorId = post.CreatedBy.Value,
                        request.HashId,
                        PostPermission = (int)PostPermission.Public,
                        Hide = request.Hides,
                        request.UserId
                    });

            var dbFeed = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var items = MapTopSeries(dbFeed.ToList(), request.UserId);
            if (items != null && items.Count() > 0)
            {
                var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
                var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Document.""DocumentPostReactions"""), new
                {
                    TargetIds = items.Select(p => p.Id).ToList(),
                    UserId = request.UserId
                });
                var results = new PagedResponse<PostSeriesTopResponse>(totalItems, request.PageNumber, request.PageSize);
                foreach (var item in items)
                {
                    var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                    if (postReaction.Count > 0)
                    {
                        MapReactionPostSeiresTopResponse(item, postReaction);
                    }
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
    public async Task<PagedResponse<PostSeriesTopResponse>> GetTopSeriesByPage(PostType type, PostSeriesSelectedType selectedType, DocumentTopPostR loadReq)
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
                    PostStatus = (int)PostStatus.Public,
                    PostPermission = (int)PostPermission.Public,
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByTagByPage(PostType type, string tagName, DocumentTopPostR loadReq)
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
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public,
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetSeriesByUserByPage(PostType type, string profileName, DocumentTopPostR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        PagedResponse<PostSeriesTopResponse> results;
        var offset = GetOffsetSetup(ref loadReq);
        var query = GetQuerySelectPage(PostSeriesSelectedType.ByUser, profileName);
        var isMySelf = profileName == loadReq.UserName;

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    loadReq.PageSize,
                    Offet = offset,
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public,
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
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Document.""DocumentPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                loadReq.UserId
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

    public async Task<PagedResponse<PostBoxResposne>> GetPostByTagName(PostType type, DocumentPostByTagNameR input)
    {
        ValidateTotalItem(input.PageSize);
        PagedResponse<PostBoxResposne> results;
        var offset = input.PageSize * (input.PageNumber - 1);
        var query = GetQuerySelectPage(PostSeriesSelectedType.ByTag);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = input.PageSize,
                    Offet = offset,
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public,
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

    public async Task<PagedResponse<PostBoxResposne>> GetPostByUserProfileName(PostType type, DocumentPostByProFileNameR input)
    {
        ValidateTotalItem(input.PageSize);
        PagedResponse<PostBoxResposne> results;
        var offset = input.PageSize * (input.PageNumber - 1);

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
                                  p.""UserId"",
                                  GREATEST(p.""CreatedOn"", MAX(sp.""PublishDate"")) AS ""LatestCreatedOn"",
                                  u.""ProfileName"",
                                  u.""UserName"",
                                  CASE 
                                  WHEN COUNT(t.""Name"") > 0 THEN array_agg(DISTINCT t.""Name"") 
                                  ELSE NULL 
                                  END AS Tags,
                                  COUNT(pc.""Id"") as CommentCount,
                                  to_jsonb(array_agg(sp.*)) AS ""SubPostStr""
                                  FROM ""document"".""DocumentPosts""  p
                                  JOIN (
                                      SELECT DISTINCT ""PostId"" 
                                      FROM ""document"".""DocumentSubPosts"" 
                                      WHERE ""IsDelete"" = false AND ""Permission"" = @Permission
                                  ) sub_posts ON p.""Id"" = sub_posts.""PostId""
                                  JOIN identity.""Users"" u ON  p.""CreatedBy""  = u.""Id"" 
                                  LEFT JOIN document.""DocumentTagPosts"" tp on p.""Id""  = tp.""PostId"" AND tp.""IsDelete"" = false
                                  LEFT JOIN ""Tags"" t on t.""Id""  = tp.""TagId"" 
                                  LEFT JOIN ""document"".""DocumentPostComments"" pc on pc.""PostId""  = p.""Id"" 
                                  LEFT JOIN LATERAL 
                                        (
                                            SELECT sp.""PostId"",sp.""Title"",sp.""Order"",sp.""CreatedOn"", sp.""PublishDate""
                                            FROM ""document"".""DocumentSubPosts"" sp 
                                            WHERE sp.""PostId"" = p.""Id"" AND sp.""IsDelete"" = false AND sp.""PublishDate"" < @CurrentDate
                                            GROUP BY sp.""Id"", sp.""PostId"", sp.""Title"",sp.""Order""
                                            ORDER BY sp.""Order"" DESC
                                        ) sp ON sp.""PostId"" = p.""Id""    
                                  [QueryCondition]
                                  GROUP BY p.""Id"" ,u.""ProfileName"", u.""UserName"", p.""Hide"", p.""Status"", p.""ExternalResource""
                                  ORDER BY p.""CreatedOn"" desc  
                                  OFFSET @Offset
                                  LIMIT @PageSize;

                                  SELECT COUNT(*) AS TotalCount
                                  FROM ""document"".""DocumentPosts""  p
                                  JOIN (
                                      SELECT DISTINCT ""PostId"" 
                                      FROM ""document"".""DocumentSubPosts"" 
                                      WHERE ""IsDelete"" = false AND ""Permission"" = @Permission
                                  ) sub_posts ON p.""Id"" = sub_posts.""PostId""
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
                    PostStatus = StatusUtils.PostStatusInt,
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

            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentPostReactions"""), new
            {
                TargetIds = items.Select(p => p.Id).ToList(),
                input.UserId
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

    public async Task<List<PostSeriesTopResponse>> GetTopNewSeries(PostType type, DocumentRecommendedR req)
    {
        var number = req.Number;
        ValidateTotalItem(number);
        var query = GetQuerySelectPage(PostSeriesSelectedType.Recommend);

        var items = await _postRepository
                .Connection.QueryAsync<PostSeriesTopQueryDbResponse>(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    PageSize = number,
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public,
                    Hide = req.Hides
                });

        return MappingTopSeries(items);
    }

    public async Task<PostSeriesResponse> PostUpdate(DocumentPostUpdateR request)
    {
        var vr = new DocumentPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(nameof(E000), t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109));
        }

        var userId = request.UserId.Value;
        var profileId = request.ProfileId;
        var profileName = request.ProfileName;

        #region -- Validate on server --
        var post = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.HashId == request.HashId);
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (post.CreatedBy != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        #endregion

        // Delete old ThumbnailUrl and CoverUrl
        await HandleThumbnailUrlAndCoverUrl(post, true, userId);

        var thumbnailUrl = await GetPublicUrl(request.ThumbnailHashId);
        var coverUrl = await GetPublicUrl(request.CoverHashId);

        if (string.IsNullOrEmpty(request.Summary))
        {
            throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
        }

        var currentTitle = post.Title;
        var currentPermission = post.Permission;
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
        post.IsAllowDownload = request.IsAllowDownload;
        post.ModifiedOn = DateTime.UtcNow;

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
            IsCompleted = request.IsCompleted,
            IsAllowDownload = request.IsAllowDownload,
            ModifiedOn = post.ModifiedOn
        };

        await _context.SaveChangesAsync(default);

        // Update IsDelete field of ThumbnailUrl and CoverUrl
        await HandleThumbnailUrlAndCoverUrl(post, false, userId);

        if (currentTitle != request.Title || currentPermission != request.Permission)
        {
            var currentEntity = await _context.SmartLookups.FirstOrDefaultAsync(p => p.KeywordType == LookupKeywordType.Document && p.Keyword == currentTitle);
            if (currentEntity != null)
            {
                currentEntity.Keyword = request.Title;
                await _context.SaveChangesAsync(default);
            }

            _ = Task.Run(async () => await SyncUpdateToAna(post));
        }

        result.Tags = (await _tagService.UpdateTagsToPost(post.Id, request.Tags, userId)).ToArray();

        return result;
    }

    private void MappingFeedRespone(PostSeriesTopResponse item)
    {
        var totalChapterView = item.Chapters.Select(x => x.ViewCount).Sum();
        var freeChapters = item.Chapters.Where(x => x.Permission == PostPermission.Public).Count();
        var exclusiveChapters = item.Chapters.Where(x => x.UserExclusiveId.HasValue).Count();
        var totalChapters = item.Chapters.Count;
        var estimateBuyChapters = totalChapters - freeChapters - exclusiveChapters;

        item.ThumbnailUrl = item.ThumbnailUrl.AppendNameSuffix();
        item.Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0];
        item.ChapterCount = totalChapters;
        item.ViewCount = item.ViewCount + totalChapterView;
        item.TotalChapters = new ChaptersExclusiveData() { Count = totalChapters, Amount = totalChapters * Default.ChapterPrice };
        item.FreeChapters = new ChaptersExclusiveData() { Count = freeChapters, Amount = freeChapters * Default.ChapterPrice };
        item.ExclusiveChapters = new ChaptersExclusiveData() { Count = exclusiveChapters, Amount = exclusiveChapters * Default.ChapterPrice };
        item.EstimateBuyChapters = new ChaptersExclusiveData() { Count = estimateBuyChapters, Amount = estimateBuyChapters * Default.ChapterPrice };
    }

    public async Task<FavoritePostResponse> FollowPost(IdBaseR request)
    {
        var postId = request.Id;
        var userId = request.UserId;

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == userId);
        if (user == null)
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        if (!await _context.Available<DocumentPost>().AnyAsync(p => p.Id == postId))
        {
            throw new BadRequestException(ApiErrorCode.NOT_FOUND, ApiErrorMessage.NOT_FOUND);
        }

        var ett = await _context.DocumentPostFavorites
              .Where(p => p.CreatedBy == user.Id && p.PostId == postId)
              .FirstOrDefaultAsync();

        if (ett == null)
        {
            ett = new DocumentPostFavorite
            {
                PostId = postId,
                UserId = user.Id,
                CreatedBy = user.Id
            };
            await _context.DocumentPostFavorites.AddAsync(ett);
        }
        else
        {
            ett.IsDelete = !ett.IsDelete;
            _context.DocumentPostFavorites.Update(ett);
        }

        await _context.SaveChangesAsync(default);

        return new FavoritePostResponse
        {
            Id = ett.Id,
            PostId = postId,
            IsFavorite = !ett.IsDelete
        };
    }

    public async Task<PagedResponse<PostSeriesTopResponse>> GetFollowedPost(PaginatedR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        var userId = loadReq.UserId;
        PagedResponse<PostSeriesTopResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(DocumentPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostFollowedIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostFollowedCountQuery);

        var result = new PostSeriesAllTopResponse();

        var query = GetQuerySelectPage(PostSeriesSelectedType.FollowedPost);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    IsAccessPrivate = false,
                    UserId = userId,
                    PageSize = loadReq.PageSize,
                    Offet = offset,
                    Hide = loadReq.Hides,
                    PostStatus = StatusUtils.PostStatusInt,
                    PostPermission = (int)PostPermission.Public
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);

            var mappedItems = MappingTopSeries(items);
            var postIds = mappedItems.Select(x => x.Id).ToList();

            var followCounts = await _context.Available<DocumentPostFavorite>()
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

    public async Task<PagedResponse<PostSeriesTopResponse>> GetMySeries(PostType type, DocumentPostListSeriesR loadReq)
    {
        ValidateTotalItem(loadReq.PageSize);
        var userId = loadReq.UserId;
        PagedResponse<PostSeriesTopResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(DocumentPost.CreatedOn);
        }
        string topSelectPostIdQuery = "";
        string countTopQuery = PaginationCountResult;

        topSelectPostIdQuery = GetMyPostIdsQuery;
        countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostCountQuery);

        var result = new PostSeriesAllTopResponse();
        var query = GetQuerySelectPage(PostSeriesSelectedType.ByMyself, loadReq.UserName);


        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostType = (int)type,
                    IsAccessPrivate = false,
                    UserId = userId,
                    loadReq.PageSize,
                    Offet = offset,
                    Hide = loadReq.Hides,
                    PostStatus = StatusUtils.PostStatusInt,
                    ProfileName = loadReq.UserName,
                    PostPermission = (int)PostPermission.Public,
                    MySelf = true,
                });
        var items = await multi.ReadAsync<PostSeriesTopQueryDbResponse>().ConfigureAwait(false);

        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            results = new PagedResponse<PostSeriesTopResponse>(totalItems, loadReq.PageNumber, loadReq.PageSize);

            var mappedItems = MappingTopSeries(items);
            var postIds = mappedItems.Select(x => x.Id).ToList();

            var followCounts = await _context.Available<DocumentPostFavorite>()
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

    public async Task<List<MyPostSeriesResponse>> GetMyAllSeries(Guid userId)
    {
        try
        {
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
                    from ""document"".""DocumentPostComments"" pc 
                    left join ""document"".""DocumentPosts"" p on  pc.""PostId"" = p.""Id""
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
                    from ""document"".""DocumentSubPostComments"" spc 
                    left join ""document"".""DocumentSubPosts"" sp on  spc.""PostId"" = sp.""Id""
                    LEFT JOIN ""document"".""DocumentPosts""  p on p.""Id"" = sp.""PostId""
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
                        FROM ""document"".""DocumentPostComments"" pc
                        LEFT JOIN ""document"".""DocumentPosts"" p on  pc.""PostId"" = p.""Id""
                        LEFT JOIN ""document"".""DocumentPostCommentReactions"" pcr on pc.""Id"" = pcr.""TargetId""
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
                        FROM ""document"".""DocumentSubPostComments"" spc 
                        LEFT join ""document"".""DocumentSubPosts"" sp on  spc.""PostId"" = sp.""Id""
                        LEFT JOIN ""document"".""DocumentSubPostCommentReactions""  spcr ON spc.""Id"" = spcr.""TargetId""
                        LEFT JOIN ""document"".""DocumentPosts""  p on p.""Id"" = sp.""PostId""
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
        var userId = req.UserId;
        var hashIds = req.HashIds;

        var param = new
        {
            HashIds = hashIds.Split(',').ToList(),
            Hide = req.Hides,
            PostStatus = StatusUtils.PostStatusInt,
            CurrentDate = DateTime.UtcNow
        };

        var result = await _postRepository.Connection.QueryAsync<PostBoxQueryResponse>(GetPostDetailsQuery, param);
        if (result != null && result.Any())
        {
            var listPostDetails = new List<PostBoxResponse>();

            var queryGetReaction = ReactionExtension.GetReactionByTargetIdsQuery;
            var postReactionResponse = await _postRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(queryGetReaction, $@"Document.""DocumentPostReactions"""), new
            {
                TargetIds = result.Select(p => p.Id).ToList(),
                UserId = userId
            });

            var commentsResult = await GetMostCommentReaction(result.Select(p => p.HashId + "").ToList());
            var comments = await GetReactionAndMentionOfComment(commentsResult.Comments, userId);

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

                postDetails.IsCensored = req.UserName != postDetails.UserName && !req.IsAdministrator && postDetails.Status == PostStatus.Inactive;
                postDetails.IsBlur = postDetails.Status == PostStatus.Inactive || postDetails.IsMature == true;

                var totalComments = commentsResult.TotalComment?.FirstOrDefault(p => p.PostHashId == postDetails.HashId)?.TotalCommentCount;
                postDetails.Comments = new CommentPagedResults<MostReactionCommentResponse>(totalComments ?? 0, 1, 2)
                {
                    Items = comments.Where(p => p.PostHashId == postDetails.HashId),
                    TotalComments = totalComments ?? 0
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

    public UploadFileDto MappingFile(DocumentResource resources)
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
            LatestCreatedOn = x.LatestCreatedOn,
            UserId = x.UserId,
        }).ToList();
    }

    private List<PostSeriesTopResponse> MapTopSeries(IEnumerable<PostSeriesTopQueryDbResponse> posts, Guid? userId)
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
            IsCurrentUserAuthor = userId == x.UserId,
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
            case PostSeriesSelectedType.ByMyself:
                {
                    topSelectPostIdQuery = GetMyPostIdsQuery;
                    countTopQuery = countTopQuery.Replace("[WhereCountQuery]", GetMyPostCountQuery);
                    break;
                }
            case PostSeriesSelectedType.FollowedPost:
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
            .Replace("[OrderBy]", "CreatedOn");

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
    public async Task<PagedResponse<ChapterResponse>> GetChapters(string hashId, DocumentChapterListR loadReq)
    {
        PagedResponse<ChapterResponse> results;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(DocumentSubPost.Sort);
        }
        var query = GetSeriesChaptersByHashId.Replace("[OrderBy]", loadReq.OrderBy);

        //TODO Premium
        var userId = loadReq.UserId;
        query = AddWithPermission(query, userId);

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    IsAccessPrivate = false,
                    PostHashId = hashId,
                    loadReq.PageSize,
                    Offet = offset,
                    UserId = userId
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
        var postId = await _context.Available<DocumentPost>().Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        if (postId == Guid.Empty)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        return await _context.Available<DocumentSubPost>().Where(p => p.PostId == postId)
            .OrderBy(p => p.Sort)
            .Select(p => new ChapterList
            {
                Sort = p.Sort,
                Order = p.Order,
                Title = p.Title
            }).ToListAsync();
    }

    public async Task<PagedResponse<ChapterTOCResponse>> GetChaptersListSimple(DocumentHashIdR request)
    {
        PagedResponse<ChapterTOCResponse> results;
        var query = GetSeriesChaptersSimpleByHashId;
        var statuses = request.IsAdministrator ? StatusUtils.PostStatusInt : StatusUtils.PostStatusIntPublic;

        var multi = await _postRepository
                .Connection.QueryMultipleAsync(query, new
                {
                    PostHashId = request.HashId,
                    CurrentDate = DateTime.UtcNow,
                    PostStatus = statuses,
                    UserId = request.UserId
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

    public async Task<PagedResponse<ChapterTOCExtendResponse>> GetChaptersListSimple(Guid userId, string hashId, DocumentChapterListR loadReq)
    {
        PagedResponse<ChapterTOCExtendResponse> results;
        var query = GetSeriesChaptersWithOffsetSimpleByHashId;
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(DocumentSubPost.Order);
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

    public async Task<ChapterResponse> SubPostCreate(DocumentSubPostCreateR request)
    {
        var vr = new DocumentSubPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(nameof(E000), t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109));
        }

        #region -- Validate on server --
        var post = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.HashId == request.PostHashId);
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        if (post.IsCompleted == true)
        {
            throw new ForbiddenAccessException(ApiErrorCode.POST_HAS_COMPLETED, ApiErrorMessage.POST_HAS_COMPLETED);
        }

        if (post.UserId != request.UserId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var hashIds = request?.Files.Select(x => x.HashId).ToList();
        var resourceList = await _context.DocumentResources.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
        if (resourceList.Count == 0)
        {
            throw new BadRequestException(nameof(E201), E201);
        }

        var hasSubPost = await _context.Available<DocumentSubPost>().AnyAsync(p => p.PostId == post.Id && p.Order == request.Order);
        if (hasSubPost)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }
        #endregion

        var newOrder = await DetermineOrder(post.Id, request.Order, request.IsAutoGenerateOrder);

        var userId = request.UserId.Value;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var userName = request.UserName;

        //Check first post
        var rewards = await CheckRewardsForSubPost(userId);

        var subPost = new DocumentSubPost
        {
            AuthorId = post.AuthorId,
            CreatedBy = userId,
            Permission = request.Permission,
            Order = newOrder,
            Name = string.IsNullOrEmpty(request.Name) ? newOrder.ToString() : request.Name,
            PostId = post.Id,
            Status = request.Status,
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
            Sort = !await _context.Available<DocumentSubPost>().AnyAsync(p => p.PostId == post.Id) ? 1 :
                    await _context.Available<DocumentSubPost>().Where(p => p.PostId == post.Id).MaxAsync(p => p.Sort) + 1,
            IsAllowDownload = request.IsAllowDownload
        };

        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = userId;

        var isPremium = subPost.Permission == PostPermission.Premium;
        if ((subPost.Permission == PostPermission.Public || isPremium) && request.IsPublicNow)
        {
            subPost.IsPublishChapterSent = await SendAddSubPostNotificationAsync(subPost, post, isPremium);
        }

        await _context.DocumentSubPosts.AddAsync(subPost);
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
            result.Files = await _fileService.ProcessDocumentFilesAsync(urDto);
        }

        _ = Task.Run(async () => await SyncCreateSubToAna(subPost));

        return result;
    }

    public async Task<ChapterResponse> SubPostUpdate(DocumentSubPostUpdateR request)
    {
        var vr = new DocumentSubPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(nameof(E000), t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109));
        }

        if (!request.IsPublicNow && request.PublishDate == null)
        {
            throw new BadRequestException(ApiErrorCode.POST_DATE_PUBLISH_NULL, ApiErrorMessage.POST_DATE_PUBLISH_NULL);
        }

        #region -- Validate on server --
        var post = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.HashId == request.PostHashId);
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        if (post.UserId != request.UserId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var subPost = await FindSubPost(request.PostHashId, request.ChapterOrder);
        if (subPost == null)
        {
            throw new NotFoundException(nameof(E208), E208);
        }

        var hasSubPost = await _context.Available<DocumentSubPost>().AnyAsync(p => p.PostId == post.Id && p.Order == request.Order && p.Id != subPost.Id);
        if (hasSubPost)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_EXISTED, ApiErrorMessage.CHAPTER_EXISTED);
        }

        var hashIds = request?.Files.Select(x => x.HashId).ToList();
        var resourceList = await _context.DocumentResources.Where(p => hashIds.Contains(p.HashId)).ToListAsync();
        if (resourceList.Count == 0)
        {
            throw new BadRequestException(nameof(E201), E201);
        }
        #endregion

        var oldOrder = subPost.Order;
        var newOrder = request.Order ?? request.ChapterOrder;

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
        var currentPermission = subPost.Permission;
        var currentIsPremium = subPost.IsPremium;

        subPost.ModifiedOn = DateTime.UtcNow;
        subPost.ModifiedBy = userId;
        //subPost.CreatorNote = request.CreatorNote;
        subPost.IsEnableComment = request.IsEnableComment;
        subPost.Permission = request.Permission;
        subPost.IsPremium = request.IsPremium;
        subPost.PostHashId = request.PostHashId;
        subPost.Order = newOrder;
        subPost.Sort = subPost.Sort;
        subPost.Status = subPost.Status == PostStatus.Draft ? request.Status : subPost.Status;
        post.ModifiedOn = DateTime.UtcNow;
        post.ModifiedBy = userId;
        subPost.IsAllowDownload = request.IsAllowDownload;

        await _context.SaveChangesAsync(default);

        if (oldOrder != newOrder && resourceList.Count > 0)
        {
            var minioInstance = MinioInstanceType.Document;
            var oldSubFolder = userFolder.GetSubFolderPath(request.PostHashId, oldOrder);
            var newSubFolder = userFolder.GetSubFolderPath(request.PostHashId, newOrder);

            try
            {
                foreach (var resource in resourceList)
                {
                    var oldObjectName = resource.Url;

                    if (!oldObjectName.Contains(oldSubFolder))
                    {
                        continue;
                    }

                    var newObjectName = oldObjectName.Replace(oldSubFolder, newSubFolder);
                    var stream = await _sc.GetStrategy(minioInstance).GetObject(oldObjectName, resource.BucketName);

                    if (stream == null)
                    {
                        continue;
                    }

                    await _sc.GetStrategy(minioInstance).PutObject(stream, newObjectName, resource.BucketName);
                    await _sc.GetStrategy(minioInstance).RemoveObject(oldObjectName, resource.BucketName);
                    resource.Url = newObjectName;
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }

            await _context.SaveChangesAsync(default);
        }

        var result = MappingChapterResponse(subPost);
        if (request?.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, subPost.PostId, subPost.PostHashId)
            {
                SubPostId = subPost.Id,
                Order = subPost.Order
            };
            result.Files = await _fileService.ProcessDocumentFilesAsync(urDto);
        }

        if (currentPermission != subPost.Permission || currentIsPremium != subPost.IsPremium)
        {
            _ = Task.Run(async () => await SyncUpdateSubToAna(subPost));
        }

        return result;
    }

    public async Task<bool> DeleteChapter(string hashId, float order, BaseR request)
    {
        var userId = request.UserId;
        var profileName = request.ProfileName;

        var subPost = await FindSubPost(hashId, order);
        if (subPost == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, order));
        }
        else if (subPost.UserId != userId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }
        else
        {
            await _postRepository.Connection.QueryAsync(ExecSoftDeleteSubPost, new { SubPostId = subPost.Id, Date = DateTime.UtcNow, UserId = userId });

            await _smartLookupService.CalculateSmartLookupWhenDeletePostAsync(subPost.PostId, profileName);

            _ = Task.Run(async () => await SyncDeleteSubToAna(subPost.Id));

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

    public ChapterResponse MappingChapterResponse(DocumentSubPost newChapter)
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
        result.IsAllowDownload = newChapter.IsAllowDownload;
        //When new, return 0
        result.CommentCount = 0;
        return result;
    }

    public async Task<List<ChapterResponse>> SwapChapterOrder(string hashId, DocumentChapterOrderSwapR orders)
    {
        var result = new List<ChapterResponse>();
        var userId = orders.UserId;
        var postId = await _context.Available<DocumentPost>().Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        var fromOrder = orders.Order1;
        var toOrder = orders.Order2;
        var chapterFr = await _context.Available<DocumentSubPost>().Where(x => x.Sort == orders.Order1 && x.PostId == postId).FirstOrDefaultAsync();
        var chapterTo = await _context.Available<DocumentSubPost>().Where(x => x.Sort == orders.Order2 && x.PostId == postId).FirstOrDefaultAsync();
        if (chapterFr == null || chapterTo == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, chapterFr == null ? orders?.Order1 : orders?.Order2));
        }

        // Check owner
        if (chapterFr.UserId != userId || chapterTo.UserId != userId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }

        if (fromOrder < toOrder)
        {
            await _context.Available<DocumentSubPost>().Where(c => c.Sort > fromOrder && c.Sort <= toOrder && c.PostId == postId)
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort - 1));
        }
        else
        {
            await _context.Available<DocumentSubPost>().Where(c => c.Sort < fromOrder && c.Sort >= toOrder && c.PostId == postId)
        .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort + 1));
        }

        chapterFr.Sort = orders.Order2;

        await _context.SaveChangesAsync(default);
        return result;
    }

    public async Task MoveChapterOrder(string hashId, DocumentChapterOrderSwapR orders)
    {
        var userId = orders.UserId;
        var postId = await _context.Available<DocumentPost>().Where(p => p.HashId == hashId).Select(p => p.Id).FirstOrDefaultAsync();
        var fromOrder = orders.Order1;
        var toOrder = orders.Order2;
        var chapterFr = await _context.Available<DocumentSubPost>().Where(x => x.Sort == orders.Order1 && x.PostId == postId).FirstOrDefaultAsync();
        var chapterTo = await _context.Available<DocumentSubPost>().Where(x => x.Sort == orders.Order2 && x.PostId == postId).FirstOrDefaultAsync();
        if (chapterFr == null || chapterTo == null)
        {
            throw new BadRequestException(ApiErrorCode.CHAPTER_NOT_EXIST, string.Format(ApiErrorMessage.CHAPTER_NOT_EXIST, chapterFr == null ? orders?.Order1 : orders?.Order2));
        }

        // Check owner
        if (chapterFr.UserId != userId || chapterTo.UserId != userId)
        {
            throw new BadRequestException(nameof(E309), E309);
        }

        if (fromOrder < toOrder)
        {
            await _context.Available<DocumentSubPost>().Where(c => c.Sort > fromOrder && c.Sort < toOrder && c.PostId == postId)
           .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort - 1));
        }
        else
        {
            await _context.Available<DocumentSubPost>().Where(c => c.Sort >= toOrder && c.Sort < fromOrder && c.PostId == postId)
          .ExecuteUpdateAsync(s => s.SetProperty(p => p.Sort, p => p.Sort + 1));
        }

        chapterFr.Sort = fromOrder < toOrder ? toOrder - 1 : toOrder;

        await _context.SaveChangesAsync(default);
    }
    #endregion

    private int GetOffsetSetup(ref DocumentTopPostR loadReq)
    {
        var offset = loadReq.PageSize * (loadReq.PageNumber - 1);

        if (loadReq.OrderBy == null)
        {
            loadReq.OrderBy = nameof(DocumentPost.CreatedOn);
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

        var check = await _context.Available<DocumentPost>().FirstOrDefaultAsync(p => p.UserId == userId && p.Type == type);
        if (check == null)
        {
            var rewardType = RewardType.FirstFeed;
            switch (type)
            {
                case PostType.Story:
                    rewardType = RewardType.FirstStory;
                    break;

                case PostType.Document:
                    rewardType = RewardType.FirstDocument;
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
                               FROM ""document"".""DocumentSubPosts"" sp
                               JOIN ""document"".""DocumentPosts""  p ON sp.""PostId"" = p.""Id""
                               JOIN ""document"".""DocumentResources"" r ON sp.""Id"" = r.""SubPostId""
                               WHERE p.""IsDelete"" = false
                               AND sp.""IsDelete"" = false
                               [QueryByType]
                               [IgnoreQuery]
                               AND sp.""Order"" = (
                                                    SELECT MIN(sp_inner.""Order"")
                                                    FROM ""document"".""DocumentSubPosts"" sp_inner
                                                    WHERE sp_inner.""PostId"" = sp.""PostId""
                                                    AND sp_inner.""IsDelete"" = false
                                                    )
                               ORDER BY RANDOM()
                               LIMIT @PageSize";
            query = query.Replace("[QueryByType]", input.IsGetAllType ? "" : $@"AND p.""Type"" = {(int)PostType.Feed}");
            query = query.Replace("[IgnoreQuery]", input.PostRandomIds == null ? "" : $@"AND NOT sp.""HashId"" = ANY(@PostRandomIds)");
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

    public async Task<IEnumerable<string>> GetPostRandomIdsAsync(PostRandomIdsR input)
    {
        try
        {
            var query = @$"SELECT ""HashId"" From ""document"".""DocumentPosts""
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

    /// <summary>
    /// GetPublicUrl
    /// </summary>
    /// <param name="hashId"></param>
    /// <returns></returns>
    private async Task<string> GetPublicUrl(string hashId)
    {
        var resource = await _context.DocumentResources.Where(p => p.HashId == hashId)
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
    /// Determines the order for a document subpost.
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

        var orders = await _context.Available<DocumentSubPost>().Where(p => p.PostId == postId).Select(p => p.Order).ToListAsync();

        return orders.Count > 0 ? (int)orders.Max() + 1 : 1;
    }

    /// <summary>
    /// Finds a specific subpost based on the parent post's hash ID and the subpost's order.
    /// </summary>
    /// <param name="postHashId">The hash ID of the parent post.</param>
    /// <param name="order">The order of the subpost.</param>
    /// <returns>The matching subpost if found; otherwise, null.</returns>
    private async Task<DocumentSubPost?> FindSubPost(string? postHashId, float order)
    {
        var q = from a in _context.Available<DocumentSubPost>()
                join b in _context.Available<DocumentPost>() on a.PostId equals b.Id
                where b.HashId == postHashId && a.Order == order
                select a;
        return await q.FirstOrDefaultAsync();
    }

    public async Task<List<RewardDto>> CheckRewardsForSubPost(Guid userId)
    {
        var res = new List<RewardDto>();

        var check = await _context.DocumentSubPosts.FirstOrDefaultAsync(p => p.UserId == userId);
        if (check == null)
        {
            var rewardType = RewardType.FirstDocument;
            res.Add(new RewardDto
            {
                Type = rewardType,
                MessageCode = rewardType.ToString()
            });
        }

        return res;
    }

    /// <summary>
    /// Handles the update or delete of thumbnail and cover resource URLs in the database.
    /// </summary>
    /// <param name="post">The document post containing the ThumbnailUrl and CoverUrl.</param>
    /// <param name="isDelete">A boolean indicating whether to mark the resources as deleted or active.</param>
    /// <param name="userId">The ID of the user performing the deletion.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task HandleThumbnailUrlAndCoverUrl(DocumentPost post, bool isDelete, Guid userId)
    {
        var resourceHashIds = new string[]
        {
            post.ThumbnailUrl!.GetResourceHashId(),
            post.CoverUrl!.GetResourceHashId()
        }.Where(p => !string.IsNullOrWhiteSpace(p));

        if (resourceHashIds.Any())
        {
            await _context.DocumentResources
                .Where(p => resourceHashIds.Contains(p.HashId))
                .ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.IsDelete, isDelete)
                    .SetProperty(p => p.ModifiedBy, userId)
                    .SetProperty(p => p.ModifiedOn, DateTime.UtcNow));
        }
    }

    private async Task<CommentsResult> GetMostCommentReaction(List<string> hashIds)
    {
        var res = new CommentsResult();

        var paramValues = new
        {
            HashIds = hashIds,
        };
        var schema = "document";
        var @params = "@HashIds";

        var fn = "document.fw_get_most_reaction_comments";
        res.Comments = await _postRepository.Connection.QueryAsync<MostReactionCommentResponse>(fn.ToFn(schema, schema, @params), paramValues);

        fn = "document.fw_get_total_comment_counts";
        res.TotalComment = await _postRepository.Connection.QueryAsync<CommentCount>(fn.ToFn(schema, schema, @params), paramValues);

        return res;
    }

    private async Task<IEnumerable<MostReactionCommentResponse>> GetReactionAndMentionOfComment(IEnumerable<MostReactionCommentResponse>? comments, Guid? userId)
    {
        if (comments == null)
        {
            return [];
        }

        var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentPostCommentReactions""");
        var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
        {
            TargetIds = comments.Where(p => p.Order == null).Select(p => p.Id).ToList(),
            UserId = userId
        });

        var querySubPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentSubPostCommentReactions""");
        var subPostCommentReactionResponse = await _subPostCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(querySubPostCommentReaction, new
        {
            TargetIds = comments.Where(p => p.Order != null).Select(p => p.Id).ToList(),
            UserId = userId
        });

        var body = "";
        foreach (var i in comments)
        {
            body += i.Body + " ";
        }
        var profiles = await _businessText.GetProfiles(body);

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

            comment.Body = await _businessText.Process(comment.Body, profiles);
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
            Reactions = reactions.Where(p => p.Type != null).Select(x => new ReactionResponse { Count = x.Count, Type = x.Type!.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    #region -- Post --
    private async Task<DocumentCreateRsp> SyncCreateToAna(DocumentPost ett)
    {
        var res = new DocumentCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentProto.DocumentProtoClient(channel);

            var request = new DocumentCreateReq
            {
                Items =
                {
                    new DocumentProtoDto
                    {
                        PostId = ett.Id.ToString(),
                        HashId = ett.HashId,
                        UserId = ett.UserId.ToString(),
                        Title = ett.Title,
                        Permission = (int)ett.Permission,
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy == null ? null : ett.CreatedBy.ToString(),
                        ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                        ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString()
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

    private async Task<DocumentUpdateRsp> SyncUpdateToAna(DocumentPost ett)
    {
        var res = new DocumentUpdateRsp() { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentProto.DocumentProtoClient(channel);

            var request = new DocumentUpdateReq
            {
                PostId = ett.Id.ToString(),
                Title = ett.Title,
                Permission = (int)ett.Permission,
                ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString()
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

    private async Task<DocumentDeleteRsp> SyncDeleteToAna(Guid id)
    {
        var res = new DocumentDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentProto.DocumentProtoClient(channel);

            var request = new DocumentDeleteReq
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
    private async Task<DocumentSubCreateRsp> SyncCreateSubToAna(DocumentSubPost ett)
    {
        var res = new DocumentSubCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentSubProto.DocumentSubProtoClient(channel);

            var request = new DocumentSubCreateReq
            {
                Items =
                {
                    new DocumentSubProtoDto
                    {
                        PostId = ett.PostId.ToString(),
                        SubPostId = ett.Id.ToString(),
                        UserId = ett.UserId.ToString(),
                        Permission = (int)ett.Permission,
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy == null ? null : ett.CreatedBy.ToString(),
                        ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                        ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString(),
                        IsPremium = ett.IsPremium
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

    private async Task<DocumentSubUpdateRsp> SyncUpdateSubToAna(DocumentSubPost ett)
    {
        var res = new DocumentSubUpdateRsp() { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentSubProto.DocumentSubProtoClient(channel);

            var request = new DocumentSubUpdateReq
            {
                SubPostId = ett.Id.ToString(),
                Permission = (int)ett.Permission,
                ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString(),
                IsPremium = ett.IsPremium
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

    private async Task<DocumentSubDeleteRsp> SyncDeleteSubToAna(Guid id)
    {
        var res = new DocumentSubDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new DocumentSubProto.DocumentSubProtoClient(channel);

            var request = new DocumentSubDeleteReq
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

    private async Task<bool> AddSubPosttionNotificationAsync(NotificationAddSubPostR req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/AddSubPost");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> SendAddSubPostNotificationAsync(DocumentSubPost subPost, DocumentPost post, bool isPremium)
    {
        var followerUserIds = await _context.Available<NotificationObject>(false)
                .Where(p => p.LocationId == subPost.PostId && p.Action == NotificationAction.FollowPost)
                .Select(p => p.ActorId)
                .Distinct()
                .ToListAsync();

        if (isPremium && followerUserIds.Count > 0)
        {
            followerUserIds = await _context.UserAvailable
                .Where(p => p.IsPremium && followerUserIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
        }

        if (followerUserIds.Count <= 0)
        {
            return false;
        }

        var notiReq = new NotificationAddSubPostR
        {
            FollowerUserIds = followerUserIds,
            PostType = post.Type,
            AuthorId = subPost.UserId,
            PostId = subPost.PostId,
            PostName = post.Title,
            SubPostId = subPost.Id,
            PostHashId = post.HashId,
            PostThumbnailUrl = post.ThumbnailUrl,
            Order = subPost.Order
        };

        await AddSubPosttionNotificationAsync(notiReq);

        return true;
    }
    #endregion

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

    private readonly IBusinessText _businessText;
    private readonly IRepository<DocumentPost> _postRepository;
    private readonly IRepository<DocumentPostComment> _postCommentRepository;
    private readonly IRepository<DocumentSubPost> _subPostRepository;
    private readonly IRepository<SocialSubPostComment> _subPostCommentRepository;
    private readonly ITagService _tagService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly ISmartLookupService _smartLookupService;

    #endregion
}
