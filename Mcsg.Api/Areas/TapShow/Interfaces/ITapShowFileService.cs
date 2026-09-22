namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Mcsg.Api.Areas.TapShow.Dtos;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Uploads for the TapShow area: images (post thumbnail, segment image, character avatar) and segment voice-over audio
/// </summary>
public interface ITapShowFileService
{
    Task<UploadFileDto> UploadImageAsync(FileCreateR request);
    Task<UploadFileDto> UploadAudioAsync(FileCreateR request);
}
