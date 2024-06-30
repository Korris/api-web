namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Entities.Common;

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
