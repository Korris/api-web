namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Common.Domain.Entities;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Characters (name + avatar) of a TapShow post: owner CRUD, public list,
/// plus the inline creation used by POST api/tapshow/tapshow
/// </summary>
public interface ITapShowCharacterService
{
    Task<CharacterResponse> CreateAsync(CharacterCreateR request);
    Task<CharacterResponse> UpdateAsync(CharacterUpdateR request);
    Task<bool> DeleteAsync(Guid id, Guid? userId);
    Task<List<CharacterResponse>> ListByPostAsync(PostHashIdR request);

    /// <summary>
    /// Characters of a post by internal id (caller already checked visibility). AvatarHashId only when isOwner.
    /// </summary>
    Task<List<CharacterResponse>> ListByPostIdAsync(Guid postId, bool isOwner);

    /// <summary>
    /// Create the inline "characters" array of POST api/tapshow/tapshow under the tracked (unsaved) post.
    /// Does NOT SaveChanges: the caller commits them together with the post. null → nothing.
    /// </summary>
    Task AddInlineAsync(TapShowPost post, List<CharacterItemR>? items, Guid userId);
}
