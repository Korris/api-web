namespace Mcsg.Api.Areas.Story.Interfaces;

using Common.Domain.Dtos;
using Mcsg.Api.Areas.Story.Dtos;
using Mcsg.Api.Areas.Story.Requests;

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
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    Task<List<SubUploadFileDto>> ProcessFilesAsync(UploadResourceDto dto);

    /// <summary>
    /// UpdateFiles async
    /// </summary>
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    Task<List<SubUploadFileDto>> UpdateFilesAsync(UploadResourceDto dto);

    /// <summary>
    /// RemoveFile async
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <param name="userFolder">User folder</param>
    /// <returns>Return the result</returns>
    Task RemoveFileAsync(Guid postId, string userFolder);
}
