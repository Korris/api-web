namespace Mcsg.Api.Areas.Story.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Dtos;
using Mcsg.Api.Areas.Story.Requests;

public interface ISoundService
{
    Task<PagedResponse<BackgroundMedia.SearchDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
    Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
    Task<BackgroundMedia.SearchDto> GetSoundByPostAsync(Guid postId);
    Task<PagedResponse<BackgroundMedia.SearchDto>> SearchSoundAsync(SoundSearchSoundR req);
    Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId);
    Task<bool> RemoveSoundAsync(Guid postId);
}
