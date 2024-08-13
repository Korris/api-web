namespace Mcsg.Social.Api.Interfaces;

using Dtos;
using Requests;

/// <summary>
/// Interface FileService
/// </summary>
public interface IFileService
{
    /// <summary>
    /// UploadImage async
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    Task<UploadFileDto> UploadImageAsync(FileCreateR request);

    /// <summary>
    /// UploadFile async
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    Task<UploadFileDto> UploadFileAsync(FileCreateR request);

    /// <summary>
    /// ProcessFiles async
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">User folder</param>
    /// <param name="userAvatar">User avatar</param>
    /// <param name="userName">UserName</param>
    /// <param name="postId">PostId</param>
    /// <param name="postHashId">PostHashId</param>
    /// <returns>Return the result</returns>
    Task<List<SubUploadFileDto>> ProcessFilesAsync(List<ResourcePostDto> req, Guid userId, string userFolder, string userAvatar, string userName, Guid postId, string postHashId);

    /// <summary>
    /// UpdateFiles async
    /// </summary>
    /// <param name="req">Request</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">User folder</param>
    /// <param name="userAvatar">User avatar</param>
    /// <param name="userName">UserName</param>
    /// <param name="postId">PostId</param>
    /// <param name="postHashId">PostHashId</param>
    /// <returns>Return the result</returns>
    Task<List<SubUploadFileDto>> UpdateFilesAsync(List<ResourcePostDto> req, Guid userId, string userFolder, string userAvatar, string userName, Guid postId, string postHashId);

    /// <summary>
    /// RemoveFile async
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <param name="userFolder">User folder</param>
    /// <returns>Return the result</returns>
    Task RemoveFileAsync(Guid postId, string userFolder);
}
