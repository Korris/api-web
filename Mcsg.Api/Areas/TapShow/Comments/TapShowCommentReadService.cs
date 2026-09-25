using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using static Common.SeedWork.Constants.Error;

public interface ITapShowCommentReadService
{
    Task<PagedResponse<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
    Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadR request);
    Task<CommentResponse> GetCommentById(Guid commentId, Guid? userId, Guid? replyCommentId);
    Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR request);
    Task<PagedResponse<MostReactionCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input);
    Task<bool> CheckPostExisted(CommentCheckPostExistedR request);

    /// <summary>
    /// Top <paramref name="take"/> most-reacted root comments of each post (post list preview, like Story list)
    /// </summary>
    Task<Dictionary<Guid, List<MostReactionCommentResponse>>> GetMostReactionCommentsOfPostsAsync(IReadOnlyCollection<Guid> postIds, Guid? userId, int take);
}

/// <summary>
/// TapShow post comments read side, cloned from Areas/Story/Services/CommentService (post comments only; TapShow has no chapter comments).
/// Writes go through the realtime CommentHub (MicroService = TapShow).
/// </summary>
public partial class TapShowCommentReadService : ITapShowCommentReadService
{
    #region -- Methods --

    public TapShowCommentReadService(IMcsgContext context, IStorageClient sc, IBusinessText businessText, IUnitOfWork unitOfWork)
    {
        _context = context;
        _sc = sc;
        _businessText = businessText;
        _connection = unitOfWork.Connection;
    }

    public async Task<PagedResponse<CommentResponse>> GetLatestPostCommentInAsync(Guid postId)
    {
        PagedResponse<CommentResponse> response;

        var result = await _connection.QueryFirstOrDefaultAsync<CommentQueryResult>(GetCommentByPostInHomePageQuery, new { PostId = postId });

        if (result != null)
        {
            response = new PagedResponse<CommentResponse>(1, 1, 1);

            var commentData = new CommentResponse()
            {
                Id = result.Id,
                AuthorId = result.AuthorId,
                AuthorName = result.AuthorName,
                UserAvatar = result.UserAvatar,
                Body = result.Body,
                PostId = result.PostId,
                CreatedOn = result.CreatedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = await _sc.GetPublicUrl(result.ResourceUrl, result.BucketName, result.MinioInstance),
                GifId = result.GifId
            };

            commentData.Body = await _businessText.Process(commentData.Body);

            var replies = new ReplyResponse()
            {
                TotalReply = (result.ReplyId != Guid.Empty) ? 1 : 0,
                Data = new List<ReplyData>()
            };

            if (result.ReplyId != Guid.Empty)
            {
                var replyData = new ReplyData()
                {
                    Id = result.ReplyId,
                    AuthorId = result.AuthorId,
                    AuthorName = result.ReplyAuthorName,
                    UserAvatar = result.ReplyUserAvatar,
                    Body = result.ReplyBody,
                    CreatedOn = result.ReplyLastCreatedDate,
                    ResourceUrl = await _sc.GetPublicUrl(result.ReplyResourceUrl, result.BucketName, result.MinioInstance),
                    ParentId = commentData.Id,
                    GifId = result.ReplyGifId,
                    QuoteId = result.ReplyQuoteId == Guid.Empty ? null : result.ReplyQuoteId
                };

                replyData.Body = await _businessText.Process(replyData.Body);

                replies.Data.Add(replyData);
            }

            commentData.Replies = replies;

            var mentions = await GetMentionsAsync(new List<Guid> { commentData.Id });
            commentData.Mentions = MapMentions(mentions, commentData.Id);

            response.Items = new List<CommentResponse>() { commentData };
            response.TotalItems = result.TotalRecord;
        }
        else
        {
            response = new PagedResponse<CommentResponse>(0);
        }
        return response;
    }

    public async Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadR request)
    {
        // Column name is interpolated into the SQL: only allow real columns
        request.OrderBy = request.OrderBy == "ModifiedOn" ? "ModifiedOn" : "CreatedOn";
        var offset = request.PageSize * (request.PageNumber - 1);

        var query = string.Format(GetCommentOfPostQuery, request.OrderBy);
        var result = (await _connection.QueryAsync<CommentQueryModel>(query, new { PostId = request.PostId })).ToList();
        var totalRecord = result.Count;
        var totalComments = 0;
        var comments = new List<CommentResponse>();
        if (result.Count > 0)
        {
            var commentModels = result.Where(x => x.CommentLevel == (int)CommentLevel.Comment).ToList();
            totalComments = commentModels.Count;
            var pagedComments = commentModels.Skip(offset).Take(request.PageSize).ToList();

            var mentions = await GetMentionsAsync(pagedComments.Select(x => x.Id).ToList());
            foreach (var comModel in pagedComments)
            {
                var comment = new CommentResponse()
                {
                    Id = comModel.Id,
                    PostId = comModel.PostId,
                    AuthorId = comModel.AuthorId,
                    AuthorName = comModel.AuthorName,
                    UserName = comModel.UserName,
                    UserAvatar = comModel.UserAvatar,
                    Body = comModel.Body,
                    CreatedOn = comModel.CreatedOn,
                    ResourceHashId = comModel.ResourceHashId,
                    ResourceUrl = await _sc.GetPublicUrl(comModel.ResourceUrl, comModel.BucketName, comModel.MinioInstance),
                    GifId = comModel.GifId,
                    CustomNote = comModel.CustomNote.ForLexical()
                };

                comment.Body = await _businessText.Process(comment.Body);

                var replyModels = result.Where(x => x.ParentId == comModel.Id
                                    && x.CommentLevel == (int)CommentLevel.Reply).ToList();

                var replies = new ReplyResponse()
                {
                    TotalReply = replyModels.Count
                };
                foreach (var repModel in replyModels)
                {
                    var replyData = new ReplyData()
                    {
                        Id = repModel.Id,
                        AuthorId = repModel.AuthorId,
                        AuthorName = repModel.AuthorName,
                        UserName = repModel.UserName,
                        UserAvatar = repModel.UserAvatar,
                        Body = repModel.Body,
                        CreatedOn = repModel.CreatedOn,
                        ResourceHashId = repModel.ResourceHashId,
                        ResourceUrl = await _sc.GetPublicUrl(repModel.ResourceUrl, repModel.BucketName, repModel.MinioInstance),
                        ParentId = comment.Id,
                        GifId = repModel.GifId,
                        QuoteId = repModel.QuoteId,
                        CustomNote = repModel.CustomNote.ForLexical()
                    };

                    replyData.Body = await _businessText.Process(replyData.Body);

                    replies.Data.Add(replyData);
                }
                comment.Replies = replies;
                comment.Mentions = MapMentions(mentions, comment.Id);

                comments.Add(comment);
            }

            var reactions = await GetCommentReactionsAsync(comments.Select(p => p.Id).ToList(), request.UserId);
            foreach (var comment in comments)
            {
                var commentReaction = reactions.Where(p => p.TargetId == comment.Id).ToList();
                if (commentReaction.Count > 0)
                {
                    comment.Reaction = MapReaction(comment.Id, commentReaction);
                }
            }
        }

        var response = new CommentPagedResults<CommentResponse>(totalRecord, request.PageNumber, request.PageSize);
        response.Items = comments;
        response.TotalComments = totalComments;
        return response;
    }

    public async Task<CommentResponse> GetCommentById(Guid commentId, Guid? userId, Guid? replyCommentId)
    {
        var query = GetCommentByIdQuery;
        var comModel = await _connection.QueryFirstOrDefaultAsync<CommentQueryModel>(query, new { CommentId = commentId });
        if (comModel == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        var result = await MapCommentResponse(comModel);
        ReplyData? replyCommentMapping = null;
        ReplyData? replyCommentQuoteMapping = null;

        var replies = new ReplyResponse()
        {
            TotalReply = comModel.ReplyCount
        };
        if (replyCommentId != null)
        {
            var replyCommentModel = await _connection.QueryFirstOrDefaultAsync<CommentQueryModel>(query, new { CommentId = replyCommentId });
            if (replyCommentModel != null)
            {
                replyCommentMapping = await MapReplyCommentResponse(replyCommentModel);
                replies.Data.Add(replyCommentMapping);
                if (replyCommentModel.QuoteId != null)
                {
                    var replyCommentQuoteModel = await _connection.QueryFirstOrDefaultAsync<CommentQueryModel>(query, new { CommentId = replyCommentModel.QuoteId });
                    if (replyCommentQuoteModel != null)
                    {
                        replyCommentQuoteMapping = await MapReplyCommentResponse(replyCommentQuoteModel);
                        replies.Data.Add(replyCommentQuoteMapping);
                        replies.Data.Reverse();
                    }
                }
            }
        }

        var targetIds = new List<Guid> { result.Id };
        if (replyCommentMapping != null)
        {
            targetIds.Add(replyCommentMapping.Id);
        }
        if (replyCommentQuoteMapping != null)
        {
            targetIds.Add(replyCommentQuoteMapping.Id);
        }

        var reactions = await GetCommentReactionsAsync(targetIds, userId);
        var reactionOfComment = reactions.Where(p => p.TargetId == result.Id).ToList();
        if (reactionOfComment.Count > 0)
        {
            result.Reaction = MapReaction(result.Id, reactionOfComment);
        }
        foreach (var reply in new[] { replyCommentMapping, replyCommentQuoteMapping })
        {
            if (reply == null)
            {
                continue;
            }
            var reactionOfReply = reactions.Where(p => p.TargetId == reply.Id).ToList();
            if (reactionOfReply.Count > 0)
            {
                reply.Reaction = MapReaction(reply.Id, reactionOfReply);
            }
        }

        result.Replies = replies;
        return result;
    }

    public async Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR request)
    {
        if (string.IsNullOrWhiteSpace(request.HashPostId))
        {
            return new CommentPagedResults<MostReactionCommentResponse>(0);
        }

        var offset = request.PageSize * (request.PageNumber - 1);
        using var multi = await _connection.QueryMultipleAsync(GetCommentWithMostReactionQuery, new
        {
            HashId = request.HashPostId,
            request.PageSize,
            Offset = offset
        });
        var items = (await multi.ReadAsync<MostReactionCommentResponse>().ConfigureAwait(false)).ToList();
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items.Count == 0)
        {
            return new CommentPagedResults<MostReactionCommentResponse>(0);
        }

        await FillMostReactionCommentsAsync(items, request.UserId);

        var results = new CommentPagedResults<MostReactionCommentResponse>(totalItems, request.PageNumber, request.PageSize);
        results.Items = items;
        results.TotalComments = await _connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = request.HashPostId });
        return results;
    }

    public async Task<PagedResponse<MostReactionCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input)
    {
        try
        {
            var offset = input.PageSize * (input.PageNumber - 1);
            using var multi = await _connection.QueryMultipleAsync(GetReplyByCommentIdQuery, new { input.CommentId, input.PageSize, Offset = offset });
            var items = (await multi.ReadAsync<MostReactionCommentResponse>().ConfigureAwait(false)).ToList();
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items.Count == 0)
            {
                return new PagedResponse<MostReactionCommentResponse>(0);
            }

            await FillMostReactionCommentsAsync(items, input.UserId);

            var results = new PagedResponse<MostReactionCommentResponse>(totalItems, input.PageNumber, input.PageSize);
            results.Items = items;
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<bool> CheckPostExisted(CommentCheckPostExistedR request)
    {
        if (request.PostId == null)
        {
            throw new BadRequestException(nameof(E500), E000);
        }

        if (request.UserId == null)
        {
            throw new UnauthorizedAccessException(nameof(E109), E109);
        }

        // TapShow has no chapter comments: IsSubPost is ignored
        return await _context.Available<TapShowPost>().AnyAsync(p => p.Id == request.PostId);
    }

    public async Task<Dictionary<Guid, List<MostReactionCommentResponse>>> GetMostReactionCommentsOfPostsAsync(IReadOnlyCollection<Guid> postIds, Guid? userId, int take)
    {
        if (postIds.Count == 0 || take < 1)
        {
            return new Dictionary<Guid, List<MostReactionCommentResponse>>();
        }

        var items = (await _connection.QueryAsync<MostReactionCommentResponse>(GetMostReactionCommentsOfPostsQuery, new
        {
            PostIds = postIds.ToList(),
            Take = take
        })).ToList();

        await FillMostReactionCommentsAsync(items, userId);

        return items.GroupBy(p => p.PostId).ToDictionary(g => g.Key, g => g.ToList());
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// Reactions, resource url, mentions and processed body (same post-processing as Story GetCommentWithMostReaction)
    /// </summary>
    private async Task FillMostReactionCommentsAsync(List<MostReactionCommentResponse> items, Guid? userId)
    {
        if (items.Count == 0)
        {
            return;
        }

        var ids = items.Select(p => p.Id).ToList();
        var reactions = await GetCommentReactionsAsync(ids, userId);
        var mentions = await GetMentionsAsync(ids);

        var body = string.Join(" ", items.Select(p => p.Body));
        var profiles = await _businessText.GetProfiles(body);

        foreach (var item in items)
        {
            var commentReaction = reactions.Where(p => p.TargetId == item.Id).ToList();
            if (commentReaction.Count > 0)
            {
                item.Reaction = MapReaction(item.Id, commentReaction);
            }
            item.ResourceUrl = await _sc.GetPublicUrl(item.ResourceUrl, item.BucketName, item.MinioInstance);
            item.Mentions = MapMentions(mentions, item.Id);
            item.Body = await _businessText.Process(item.Body, profiles);
        }
    }

    private async Task<CommentResponse> MapCommentResponse(CommentQueryModel queryModel)
    {
        return new CommentResponse()
        {
            Id = queryModel.Id,
            PostId = queryModel.PostId,
            AuthorId = queryModel.AuthorId,
            AuthorName = queryModel.AuthorName,
            UserName = queryModel.UserName,
            UserAvatar = queryModel.UserAvatar,
            Body = await _businessText.Process(queryModel.Body),
            CreatedOn = queryModel.CreatedOn,
            ResourceHashId = queryModel.ResourceHashId,
            ResourceUrl = await _sc.GetPublicUrl(queryModel.ResourceUrl, queryModel.BucketName, queryModel.MinioInstance),
            GifId = queryModel.GifId,
            CustomNote = queryModel.CustomNote.ForLexical(),
            ReplyCount = queryModel.ReplyCount,
        };
    }

    private async Task<ReplyData> MapReplyCommentResponse(CommentQueryModel queryModel)
    {
        return new ReplyData()
        {
            Id = queryModel.Id,
            AuthorId = queryModel.AuthorId,
            AuthorName = queryModel.AuthorName,
            UserName = queryModel.UserName,
            ParentId = queryModel.ParentId,
            UserAvatar = queryModel.UserAvatar,
            Body = await _businessText.Process(queryModel.Body),
            CreatedOn = queryModel.CreatedOn,
            ResourceHashId = queryModel.ResourceHashId,
            ResourceUrl = await _sc.GetPublicUrl(queryModel.ResourceUrl, queryModel.BucketName, queryModel.MinioInstance),
            GifId = queryModel.GifId,
            CustomNote = queryModel.CustomNote.ForLexical(),
            QuoteId = queryModel.QuoteId,
            PostId = queryModel.PostId
        };
    }

    private async Task<List<CommentReactionResponseQuery>> GetCommentReactionsAsync(List<Guid> targetIds, Guid? userId)
    {
        if (targetIds.Count == 0)
        {
            return new List<CommentReactionResponseQuery>();
        }
        var query = string.Format(GetReactionByTargetIdsQuery, CommentReactionTable);
        return (await _connection.QueryAsync<CommentReactionResponseQuery>(query, new { TargetIds = targetIds, UserId = userId })).ToList();
    }

    private async Task<List<UserMentionModel>> GetMentionsAsync(List<Guid> locationIds)
    {
        if (locationIds.Count == 0)
        {
            return new List<UserMentionModel>();
        }
        return (await _connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = locationIds })).ToList();
    }

    private static List<UserMentionResponse> MapMentions(List<UserMentionModel> mentions, Guid locationId)
    {
        return mentions.Where(x => x.LocationId == locationId)
            .Select(x => new UserMentionResponse
            {
                UserId = x.EntityId,
                ProfileName = x.ProfileName,
                UserName = x.UserName,
                Length = x.Length,
                Offset = x.Offset
            })
            .ToList();
    }

    private static ReactionsResponse MapReaction(Guid targetId, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.FirstOrDefault(x => x.ReactByCurrent > 0);
        return new ReactionsResponse
        {
            TargetId = targetId,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Where(x => x.Type != null).Select(x => new ReactionResponse { Count = x.Count, Type = x.Type!.Value }).ToList(),
            TotalReacts = reactions.Sum(x => x.Count),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly IStorageClient _sc;
    private readonly IBusinessText _businessText;
    private readonly System.Data.IDbConnection _connection;

    #endregion
}
