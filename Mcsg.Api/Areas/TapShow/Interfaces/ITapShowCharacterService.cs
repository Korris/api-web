namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Characters (name + avatar) of a TapShow post: owner CRUD, public list
/// </summary>
public interface ITapShowCharacterService
{
    Task<CharacterResponse> CreateAsync(CharacterCreateR request);
    Task<CharacterResponse> UpdateAsync(CharacterUpdateR request);
    Task<bool> DeleteAsync(Guid id, Guid? userId);
    Task<List<CharacterResponse>> ListByPostAsync(PostHashIdR request);
}
