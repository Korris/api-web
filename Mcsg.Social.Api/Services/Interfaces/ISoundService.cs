using Mcsg.Social.Api.DTOs;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface ISoundService
    {
        Task<PagedResults<SoundDto>> GetAllSoundAsync(BackgroundMediaLoadReq req);
        Task<PagedResults<SoundRecentlyDto>> GetRecentlyUseSoundAsync(BackgroundMediaLoadReq req);
        Task<SoundDto> GetSoundByPostAsync(Guid postId);
        Task<PagedResults<SoundDto>> SearchSoundAsync(SearchSoundReq req);
        Task<bool> AddSoundAsync(Guid postId, Guid soundId);
        Task<bool> RemoveSoundAsync(Guid postId);
    }
}
