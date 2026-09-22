using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Areas.TapShow.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Characters of a TapShow post. Writes are owner-only; the list follows the post visibility rule.
/// Deleting a character detaches it from its segments (CharacterId → null) and releases its avatar.
/// </summary>
public class TapShowCharacterService : ITapShowCharacterService
{
    #region -- Methods --

    public TapShowCharacterService(IMcsgContext context, ITapShowResourceService resources)
    {
        _context = context;
        _resources = resources;
    }

    public async Task<CharacterResponse> CreateAsync(CharacterCreateR request)
    {
        var vr = new CharacterCreateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);

        var post = await _context.Available<TapShowPost>().FirstOrDefaultAsync(p => p.HashId == request.PostHashId)
                   ?? throw new NotFoundException(nameof(E204), E204);
        if (post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var avatar = string.IsNullOrWhiteSpace(request.AvatarHashId) ? null : await _resources.GetTempResourceAsync(request.AvatarHashId, userId);
        var order = request.Order ?? await NextOrderAsync(post.Id);

        var character = TapShowCharacter.Create(post.Id, request.Name!.Trim(), avatar == null ? null : _resources.PublicUrl(avatar), order, userId);
        await _context.TapShowCharacters.AddAsync(character);
        if (avatar != null)
        {
            _resources.Attach(avatar, post, character: character);
        }
        await _context.SaveChangesAsync(default);

        return await GetByIdAsync(character.Id, true);
    }

    public async Task<CharacterResponse> UpdateAsync(CharacterUpdateR request)
    {
        var vr = new CharacterUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var character = await GetOwnedCharacterAsync(request.Id, userId);

        // Avatar: same hashId → keep; new hashId → swap; null → remove
        var current = character.TapShowResources.FirstOrDefault(r => !r.IsDelete);
        var avatar = current;
        if (string.IsNullOrWhiteSpace(request.AvatarHashId))
        {
            avatar = null;
        }
        else if (current == null || current.HashId != request.AvatarHashId)
        {
            avatar = await _resources.GetTempResourceAsync(request.AvatarHashId, userId);
        }
        if (current != null && avatar != current)
        {
            _resources.Release(current, userId);
        }

        character.Update(request.Name!.Trim(), avatar == null ? null : _resources.PublicUrl(avatar), request.Order ?? character.Order, userId);
        if (avatar != null)
        {
            _resources.Attach(avatar, character.Post, character: character);
        }
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return await GetByIdAsync(character.Id, true);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? currentUserId)
    {
        var userId = RequireUser(currentUserId);
        var character = await GetOwnedCharacterAsync(id, userId);

        // Segments keep their content, they just lose the speaker
        var segments = await _context.Available<TapShowSegment>().Where(s => s.CharacterId == id).ToListAsync();
        foreach (var segment in segments)
        {
            segment.CharacterId = null;
            segment.ModifiedBy = userId;
            segment.ModifiedOn = DateTime.UtcNow;
        }
        character.Delete(userId);
        foreach (var resource in character.TapShowResources.Where(r => !r.IsDelete))
        {
            _resources.Release(resource, userId);
        }
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return true;
    }

    /// <summary>
    /// Characters of a post ordered by Order. Post visibility: Public + not Private, or owner (404 otherwise).
    /// </summary>
    public async Task<List<CharacterResponse>> ListByPostAsync(PostHashIdR request)
    {
        var post = await _context.Available<TapShowPost>(false)
            .Where(p => p.HashId == request.PostHashId)
            .Select(p => new { p.Id, p.UserId, p.Status, p.Permission })
            .FirstOrDefaultAsync();
        var isOwner = post != null && request.UserId != null && post.UserId == request.UserId;
        if (post == null || (!isOwner && (post.Status != PostStatus.Public || post.Permission == PostPermission.Private)))
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        return await Project(_context.Available<TapShowCharacter>(false).Where(c => c.PostId == post.Id), isOwner).ToListAsync();
    }

    #endregion

    #region -- Helpers --

    private static Guid RequireUser(Guid? userId)
    {
        if (userId == null || userId == Guid.Empty)
        {
            throw new BadRequestException(nameof(E109), E109);
        }
        return userId.Value;
    }

    /// <summary>
    /// Character that must exist (E204) and whose post belongs to the user (E309), with post and avatar loaded
    /// </summary>
    private async Task<TapShowCharacter> GetOwnedCharacterAsync(Guid id, Guid userId)
    {
        var character = await _context.Available<TapShowCharacter>()
            .Include(c => c.Post)
            .Include(c => c.TapShowResources)
            .FirstOrDefaultAsync(c => c.Id == id && !c.Post.IsDelete);
        if (character == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (character.Post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        return character;
    }

    private async Task<CharacterResponse> GetByIdAsync(Guid id, bool isOwner)
    {
        return await Project(_context.Available<TapShowCharacter>(false).Where(c => c.Id == id), isOwner).FirstOrDefaultAsync()
               ?? throw new NotFoundException(nameof(E204), E204);
    }

    private async Task<int> NextOrderAsync(Guid postId)
    {
        var max = await _context.Available<TapShowCharacter>(false)
            .Where(c => c.PostId == postId)
            .Select(c => (int?)c.Order)
            .MaxAsync();
        return (max ?? 0) + 1;
    }

    /// <summary>
    /// AvatarHashId is only exposed to the owner
    /// </summary>
    private static IQueryable<CharacterResponse> Project(IQueryable<TapShowCharacter> query, bool isOwner)
    {
        return query
            .OrderBy(c => c.Order).ThenBy(c => c.CreatedOn)
            .Select(c => new CharacterResponse
            {
                Id = c.Id,
                PostId = c.PostId,
                Name = c.Name,
                AvatarUrl = c.AvatarUrl,
                AvatarHashId = isOwner ? c.TapShowResources.Where(r => !r.IsDelete).Select(r => r.HashId).FirstOrDefault() : null,
                Order = c.Order,
                CreatedOn = c.CreatedOn,
                ModifiedOn = c.ModifiedOn
            });
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ITapShowResourceService _resources;

    #endregion
}
