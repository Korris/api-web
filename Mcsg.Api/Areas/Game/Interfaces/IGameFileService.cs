namespace Mcsg.Api.Areas.Game.Interfaces;

using Mcsg.Api.Areas.Game.Dtos;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Uploads for the game area (thumbnail image, game .html file)
/// </summary>
public interface IGameFileService
{
    Task<UploadFileDto> UploadThumbnailAsync(FileCreateR request);
    Task<UploadFileDto> UploadGameAsync(FileCreateR request);
}
