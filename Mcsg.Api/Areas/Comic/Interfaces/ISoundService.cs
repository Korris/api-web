namespace Mcsg.Api.Areas.Comic.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Comic.Dtos;
using Mcsg.Api.Areas.Comic.Requests;

public interface ISoundService
{
    Task<PagedResponse<BackgroundMedia.SearchDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
    Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
    Task<BackgroundMedia.SearchDto> GetSoundByPostAsync(Guid postId);
    Task<PagedResponse<BackgroundMedia.SearchDto>> SearchSoundAsync(SoundSearchSoundR req);
    Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId);
    Task<bool> RemoveSoundAsync(Guid postId);
}
