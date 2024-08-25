namespace Mcsg.Comic.Api.Interfaces;

using Common.Domain.Dtos;
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
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    Task<List<SubUploadFileDto>> ProcessFilesAsync(UploadResourceDto dto);

    /// <summary>
    /// ProcessComicFiles async
    /// </summary>
    /// <param name="dto">UploadResourceDto</param>
    /// <returns>Return the result</returns>
    Task<List<UploadFileDto>> ProcessComicFilesAsync(UploadResourceDto dto);

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
