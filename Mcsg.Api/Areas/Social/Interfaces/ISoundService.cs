namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Requests;

public interface ISoundService
{
    Task<PagedResponse<BackgroundMedia.SearchDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
    Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
    Task<BackgroundMedia.SearchDto> GetSoundByPostAsync(Guid postId);
    Task<PagedResponse<BackgroundMedia.SearchDto>> SearchSoundAsync(SoundSearchSoundR req);
    Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId);
    Task<bool> RemoveSoundAsync(Guid postId);
}
