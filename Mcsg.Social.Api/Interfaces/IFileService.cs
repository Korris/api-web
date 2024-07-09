namespace Mcsg.Social.Api.Interfaces;

using Dtos;

public interface IFileService
{
    Task<UploadFileDto> UploadImageAsync(IFormFile file);
    Task<UploadFileDto> UploadFileAsync(IFormFile file);
    Task<List<SubUploadFileDto>> ProcessFeedFilesAsync(List<ResourcePostDto> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid postId, string postHashId);
    Task<List<UploadFileDto>> ProcessComicFilesAsync(List<ResourcePostDto> resourceRequest, Guid userId, string userFolder, string userAvatar, Guid subPostId);
    Task<List<SubUploadFileDto>> UpdateFeedFilesAsync(List<ResourcePostDto> resourceRequest, Guid userId, string userFolder, string userAvatar, string userName, Guid postId, string postHashId);
    Task RemoveFileAsync(Guid postId, string username);
}
