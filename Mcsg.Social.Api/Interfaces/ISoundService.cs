namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Requests;

    public interface ISoundService
    {
        Task<PagedResults<SoundDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req);
        Task<PagedResults<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req);
        Task<SoundDto> GetSoundByPostAsync(Guid postId);
        Task<PagedResults<SoundDto>> SearchSoundAsync(SoundSearchSoundR req);
        Task<bool> AddSoundAsync(Guid postId, Guid soundId);
        Task<bool> RemoveSoundAsync(Guid postId);
    }
}
