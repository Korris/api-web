namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Lib.Data.Domain.Entities;
using Requests;

public interface ISoundService
{
    Task<PagedResponse<BackgroundMedia.SearchDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
    Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
    Task<BackgroundMedia.SearchDto> GetSoundByPostAsync(Guid postId);
    Task<PagedResponse<BackgroundMedia.SearchDto>> SearchSoundAsync(SoundSearchSoundR req);
    Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId);
    Task<bool> RemoveSoundAsync(Guid postId);
}
