using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IFileService
    {
        Task<UploadFileResponse> UploadImageAsync(IFormFile file);
        Task<UploadFileResponse> UploadFileAsync(IFormFile file);
        Task<List<SubPostResponse>> ProcessFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userName, string userAvatar, Guid postId, string postHashId);
        Task<List<UploadFileResponse>> ProcessComicFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userName, string userAvatar, Guid subPostId);
        Task<List<SubPostResponse>> UpdateFeedFilesAsync(List<ResourcePostReq> resourceRequest, Guid userId, string userName, string userAvatar, Guid postId, string postHashId);
        Task RemoveFileAsync(Guid postId, string username);
    }
}
