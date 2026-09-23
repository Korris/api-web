using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Inline "characters" array of POST api/tapshow/tapshow, plus the create / update / delete
/// primitives shared with the standalone character endpoints. Nothing here calls SaveChanges.
/// </summary>
public partial class TapShowCharacterService
{
    #region -- Methods --

    public async Task AddInlineAsync(TapShowPost post, List<CharacterItemR>? items, Guid userId)
    {
        if (items == null)
        {
            return;
        }

        var usedAvatars = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];

            // One upload can only be the avatar of one character
            if (!string.IsNullOrWhiteSpace(item.AvatarHashId) && !usedAvatars.Add(item.AvatarHashId))
            {
                throw new BadRequestException(nameof(E202), TapShowConfig.InvalidResourceUrlMessage);
            }
            await AddAsync(post, item.Name!, item.AvatarHashId, item.Order ?? i, userId);
        }
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// New character under the (tracked, possibly unsaved) post; avatar resolved from a temp upload
    /// </summary>
    private async Task<TapShowCharacter> AddAsync(TapShowPost post, string name, string? avatarHashId, int order, Guid userId)
    {
        var avatar = string.IsNullOrWhiteSpace(avatarHashId) ? null : await _resources.GetTempResourceAsync(avatarHashId, userId);

        var character = TapShowCharacter.Create(post.Id, name.Trim(), avatar == null ? null : _resources.PublicUrl(avatar), order, userId);
        await _context.TapShowCharacters.AddAsync(character);
        if (avatar != null)
        {
            _resources.Attach(avatar, post, character: character);
        }
        return character;
    }

    /// <summary>
    /// Update name / order and the avatar: same hashId → keep; new hashId → swap; null → remove.
    /// Character must be loaded with its TapShowResources.
    /// </summary>
    private async Task ApplyUpdateAsync(TapShowCharacter character, TapShowPost post, string name, string? avatarHashId, int? order, Guid userId)
    {
        var current = character.TapShowResources.FirstOrDefault(r => !r.IsDelete);
        var avatar = current;
        if (string.IsNullOrWhiteSpace(avatarHashId))
        {
            avatar = null;
        }
        else if (current == null || current.HashId != avatarHashId)
        {
            avatar = await _resources.GetTempResourceAsync(avatarHashId, userId);
        }
        if (current != null && avatar != current)
        {
            _resources.Release(current, userId);
        }

        character.Update(name.Trim(), avatar == null ? null : _resources.PublicUrl(avatar), order ?? character.Order, userId);
        if (avatar != null)
        {
            _resources.Attach(avatar, post, character: character);
        }
    }

    /// <summary>
    /// Soft-delete the character: segments keep their content but lose the speaker, the avatar is released
    /// </summary>
    private async Task DetachAndDeleteAsync(TapShowCharacter character, Guid userId)
    {
        var segments = await _context.Available<TapShowSegment>().Where(s => s.CharacterId == character.Id).ToListAsync();
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
    }

    #endregion
}
