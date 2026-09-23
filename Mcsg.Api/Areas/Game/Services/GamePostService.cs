using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Game.Services;

using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Models;
using Mcsg.Api.Areas.Game.Requests;
using Mcsg.Api.Areas.Game.Validators;
using Mcsg.Api.Interfaces;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Game post create / update / delete.
/// Queries: GamePostService.Query.cs. Resource (uploaded file) handling: GamePostService.Resources.cs.
/// </summary>
public partial class GamePostService : IGamePostService
{
    #region -- Methods --

    public GamePostService(IMcsgContext context, ISetting setting, IStorageClient sc, IGameReactService reactService,
        IPostHashtagService hashtags, ILogger<GamePostService> logger)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _reactService = reactService;
        _hashtags = hashtags;
        _logger = logger;
    }

    public async Task<GamePostResponse> CreateAsync(GamePostCreateR request)
    {
        var vr = new GamePostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);

        // Same flow as Comic: files were uploaded first, the post references them by hashId
        var thumbnail = await GetTempResourceAsync(request.ThumbnailHashId, userId, ResourceKind.Thumbnail);
        var game = await GetTempResourceAsync(request.GameHashId, userId, ResourceKind.Game);

        var post = GamePost.Create(request.Title, request.Summary, PublicUrl(thumbnail), PublicUrl(game),
            request.IsCurrentUserAuthor ? request.ProfileName : request.AuthorName,
            request.IsCurrentUserAuthor ? userId : null,
            request.IsMature, request.Permission, userId);

        await _context.GamePosts.AddAsync(post);
        Attach(thumbnail, post);
        Attach(game, post);
        await _hashtags.SetAsync<GameTagPost>(post.Id, request.Tags, userId);
        await _context.SaveChangesAsync(default);

        return await GetByHashIdAsync(post.HashId, userId, true);
    }

    public async Task<GamePostResponse> UpdateAsync(GamePostUpdateR request)
    {
        var vr = new GamePostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var post = await GetOwnedPostAsync(request.HashId, userId, true);

        // Keep the current file when the same hashId is sent, otherwise swap to the new upload
        var thumbnail = await ResolveForUpdateAsync(post, request.ThumbnailHashId, userId, ResourceKind.Thumbnail);
        var game = await ResolveForUpdateAsync(post, request.GameHashId, userId, ResourceKind.Game);

        post.Update(request.Title, request.Summary, PublicUrl(thumbnail), PublicUrl(game),
            request.IsCurrentUserAuthor ? request.ProfileName : request.AuthorName,
            request.IsCurrentUserAuthor ? userId : null,
            request.IsMature, request.Permission, userId);
        Attach(thumbnail, post);
        Attach(game, post);
        // PUT carries the full tag set: names left out are unlinked
        await _hashtags.SetAsync<GameTagPost>(post.Id, request.Tags, userId);
        await _context.SaveChangesAsync(default);

        await RemoveReleasedObjectsAsync();
        return await GetByHashIdAsync(post.HashId, userId, true);
    }

    public async Task<bool> DeleteAsync(GameHashIdR request)
    {
        var userId = RequireUser(request.UserId);
        var post = await GetOwnedPostAsync(request.HashId, userId, true);

        post.Delete(userId);
        foreach (var resource in post.GameResources.Where(r => !r.IsDelete))
        {
            Release(resource, userId);
        }
        await _context.SaveChangesAsync(default);

        // Soft-deleted post must not keep its HTML publicly served
        await RemoveReleasedObjectsAsync();
        return true;
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// Logged-in user id or E109
    /// </summary>
    private static Guid RequireUser(Guid? userId)
    {
        if (userId == null || userId == Guid.Empty)
        {
            throw new BadRequestException(nameof(E109), E109);
        }
        return userId.Value;
    }

    /// <summary>
    /// Load a post that must exist (E204) and belong to the user (E309), optionally with its attached files
    /// </summary>
    private async Task<GamePost> GetOwnedPostAsync(string? hashId, Guid userId, bool includeResources)
    {
        var query = _context.Available<GamePost>();
        if (includeResources)
        {
            query = query.Include(p => p.GameResources);
        }
        var post = await query.FirstOrDefaultAsync(p => p.HashId == hashId);
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        return post;
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ISetting _setting;
    private readonly IStorageClient _sc;
    private readonly IGameReactService _reactService;
    private readonly IPostHashtagService _hashtags;
    private readonly ILogger<GamePostService> _logger;

    #endregion
}
