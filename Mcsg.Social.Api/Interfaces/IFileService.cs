namespace Mcsg.Social.Api.Interfaces
{
    using Models;
    using Requests;

    public interface IFileService
    {
        Task<UploadFileResponse> UploadImageAsync(IFormFile file);
        Task<UploadFileResponse> UploadFileAsync(IFormFile file);
        Task<List<SubPostResponse>> ProcessFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid postId, string postHashId);
        Task<List<UploadFileResponse>> ProcessComicFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid subPostId);
        Task<List<SubPostResponse>> UpdateFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userName, string userFolder, string userAvatar, Guid postId, string postHashId);
        Task RemoveFileAsync(Guid postId, string username);
    }
}
