using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.Extensions;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using static Common.SeedWork.Constants.Error;

public interface ITapShowReactionService<T> where T : BaseReaction, new()
{
    Task<ReactionUpdateResponse> AddReaction(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}

/// <summary>
/// Post / post comment reactions, cloned from Areas/Story/Services/ReactService (same requests and responses).
/// T is TapShowPostReaction or TapShowPostCommentReaction.
/// </summary>
public partial class TapShowReactionService<T> : ITapShowReactionService<T> where T : BaseReaction, new()
{
    #region -- Methods --

    public TapShowReactionService(IMcsgContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _connection = unitOfWork.Connection;
    }

    public async Task<ReactionUpdateResponse> AddReaction(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var userId = request.UserId ?? Guid.Empty;
        var type = request.Type;

        var reactionInfor = await GetReactionInfor(targetId);

        var response = new ReactionUpdateResponse
        {
            MicroService = TapShowMicroService,
            TargetId = reactionInfor.PostId,
            SubPostId = reactionInfor.SubPostId
        };

        var ett = await _context.Available<T>().FirstOrDefaultAsync(p => p.TargetId == request.TargetId && p.AuthorId == request.UserId);

        var isChange = false;
        var newReaction = false;

        if (ett != null)
        {
            response.ReactionId = ett.Id;
            if (ett.IsDelete)
            {
                ett.IsDelete = false;
                isChange = true;
            }

            if (ett.Type != type)
            {
                ett.Type = type;
                isChange = true;
            }

            if (isChange)
            {
                // Update when revert delete or update new type
                ett.ModifiedBy = userId;
                ett.ModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync(default);

                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
        }
        else
        {
            ett = new T
            {
                TargetId = targetId,
                AuthorId = userId,
                Type = type,
                CreatedBy = userId
            };
            await _context.Set<T>().AddAsync(ett);
            if (await _context.SaveChangesAsync(default) > 0)
            {
                newReaction = true;
                response.ReactionId = ett.Id;
                response.IsDeleted = ett.IsDelete;
            }
        }

        if ((isChange || newReaction) && reactionInfor.AuthorId != userId)
        {
            await OnReactedAsync(ett, request);
        }

        return response;
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        var targetId = request.TargetId;
        var query = string.Format(GetReactByTargetQuery, TableName);

        var reactionsDb = (await _connection.QueryAsync<ReactionResponseQuery>(query, new
        {
            TargetId = targetId,
            request.UserId
        })).ToList();
        var currentUserReact = reactionsDb.FirstOrDefault(x => x.ReactByCurrent > 0);
        var result = new ReactionsResponse
        {
            TargetId = targetId,
            CurrentUserReactType = currentUserReact?.Type
        };
        result.Reactions = reactionsDb.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type }).ToList();
        result.TotalReacts = reactionsDb.Sum(x => x.Count);
        if (result.TotalReacts > 0)
        {
            result.MostReactionType = reactionsDb.OrderByDescending(p => p.Count).First().Type;
        }
        return result;
    }

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        int? reactType = !request.Type.IsNumeric() ? null : request.Type.ToInt();
        var query = string.Format(GetReactionByTargetQuery, TableName);

        var offset = request.PageSize * (request.PageNumber - 1);

        using var multi = await _connection.QueryMultipleAsync(query, new
        {
            TargetId = targetId,
            Type = reactType,
            request.PageSize,
            Offet = offset
        });

        var items = (await multi.ReadAsync<ReactionsUserModel>().ConfigureAwait(false)).ToList();
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        var userId = request.UserId;
        var followingList = await _context.Available<UserFollow>()
                            .Where(p => userId == p.UserFollowerId)
                            .Select(p => p.UserFollowingId)
                            .ToListAsync();

        foreach (var item in items)
        {
            item.IsFollowing = followingList.Contains(item.AuthorId);
        }

        var response = new PagedResponse<ReactionsUserModel>(totalItems, request.PageNumber, request.PageSize);
        response.Items = items;
        return response;
    }

    public async Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request)
    {
        var reactionInfor = await GetReactionInfor(request.TargetId);

        var ett = await _context.Available<T>().FirstOrDefaultAsync(p => p.TargetId == request.TargetId && p.AuthorId == request.UserId);
        if (ett == null)
        {
            throw new NotFoundException(nameof(E002), E002);
        }

        ett.IsDelete = true;
        ett.ModifiedBy = request.UserId;
        ett.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(default);

        return new ReactionUpdateResponse
        {
            ReactionId = ett.Id,
            MicroService = TapShowMicroService,
            TargetId = reactionInfor.PostId,
            SubPostId = reactionInfor.SubPostId,
            IsDeleted = ett.IsDelete
        };
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// Hook after a new / changed reaction by someone else than the target author (notification)
    /// </summary>
    partial void OnReacted(T ett, ReactionReactR request, List<Task> tasks);

    private Task OnReactedAsync(T ett, ReactionReactR request)
    {
        var tasks = new List<Task>();
        OnReacted(ett, request, tasks);
        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Same rules as Story: post → (Guid.Empty, postId, owner); comment → (postId, Guid.Empty, comment author)
    /// </summary>
    private async Task<ReactionInfor> GetReactionInfor(Guid targetId)
    {
        switch (typeof(T).Name)
        {
            case nameof(TapShowPostReaction):
                var post = await _context.Available<TapShowPost>()
                    .Where(p => p.Id == targetId)
                    .Select(p => new { p.Id, p.UserId })
                    .FirstOrDefaultAsync();
                if (post == null)
                {
                    throw new NotFoundException(nameof(E204), E204);
                }
                return new ReactionInfor(Guid.Empty, post.Id, post.UserId);

            case nameof(TapShowPostCommentReaction):
                var comment = await _context.Available<TapShowPostComment>()
                    .Where(p => p.Id == targetId && !p.Post.IsDelete)
                    .Select(p => new { p.PostId, p.AuthorId })
                    .FirstOrDefaultAsync();
                if (comment == null)
                {
                    throw new NotFoundException(nameof(E204), E204);
                }
                return new ReactionInfor(comment.PostId, Guid.Empty, comment.AuthorId);

            default:
                return new ReactionInfor(Guid.Empty, Guid.Empty, Guid.Empty);
        }
    }

    private static string TableName => typeof(T).Name switch
    {
        nameof(TapShowPostReaction) => @"tapshow.""TapShowPostReactions""",
        nameof(TapShowPostCommentReaction) => @"tapshow.""TapShowPostCommentReactions""",
        _ => throw new NotSupportedException(typeof(T).Name)
    };

    private static string GetReactByTargetQuery => @"SELECT ""Type"",SUM(""Count"") AS ""Count"", SUM(""ReactByCurrent"")  AS ""ReactByCurrent"" FROM
                    (SELECT r.""Type"", COUNT(*) AS ""Count"", CASE
                      WHEN r.""AuthorId"" = @UserId THEN 1
                      ELSE 0
                     END AS ""ReactByCurrent""
                    FROM {0} r
                    INNER JOIN ""identity"".""Users"" u ON r.""AuthorId"" = u.""Id""
                    WHERE ""TargetId"" = @TargetId
                    AND r.""IsDelete"" = false
                    GROUP BY r.""Type"", r.""AuthorId"") react
                    GROUP BY ""Type"" 
                    ORDER BY ""Count"" DESC";

    private static string GetReactionByTargetQuery => @"SELECT r.""Type"", r.""AuthorId""
                        , (CASE WHEN u.""ProfileName"" IS NULL THEN u.""UserName""  ELSE u.""ProfileName"" END) AS ""AuthorName""
                        , u.""Avatar"" AS ""AuthorAvatar"",u.""UserName"", u.""IsDelete"" AS IsDeletedUser
                        FROM {0} r
                        INNER JOIN identity.""Users"" u ON r.""AuthorId"" = u.""Id""
                        WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false
                        AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END)
                        ORDER BY r.""ModifiedOn"" DESC 
                        LIMIT @PageSize
                        OFFSET @Offet ;

                        SELECT COUNT(""Id"") FROM {0} r
                        WHERE r.""TargetId"" = @TargetId AND r.""IsDelete"" = false
                        AND r.""Type"" = (CASE WHEN @Type IS NULL THEN r.""Type"" ELSE @Type END) ; ";

    private sealed record ReactionInfor(Guid PostId, Guid SubPostId, Guid AuthorId);

    #endregion

    #region -- Fields --

    /// <summary>
    /// MicroService name used by the realtime CommentHub / notifications for TapShow
    /// </summary>
    public const string TapShowMicroService = "TapShow";

    private readonly IMcsgContext _context;
    private readonly System.Data.IDbConnection _connection;

    #endregion
}
