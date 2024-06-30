namespace Mcsg.Social.Api.Interfaces
{
    using Models;

    public interface IPostLinkService
    {
        Task<PostLinkResponse> AddLinkAsync(Guid postId, string content);
        Task<bool> RemoveLinkAsync(Guid postId);
    }
}
