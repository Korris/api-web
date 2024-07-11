namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Requests;

public interface ISoundService
{
    Task<PagedResponse<SoundDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
    Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
    Task<SoundDto> GetSoundByPostAsync(Guid postId);
    Task<PagedResponse<SoundDto>> SearchSoundAsync(SoundSearchSoundR req);
    Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId);
    Task<bool> RemoveSoundAsync(Guid postId);
}
