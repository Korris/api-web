namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;
using Dtos;

/// <summary>
/// FormBase request
/// </summary>
public class PostFormBase : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// Title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Thumbnail URL
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Meta data
    /// </summary>
    public MetaDataDto? MetaData { get; set; }

    /// <summary>
    /// Tags
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Files
    /// </summary>
    public List<ResourcePostDto>? Files { get; set; }

    /// <summary>
    /// Sound ID
    /// </summary>
    public Guid? SoundId { get; set; }

    /// <summary>
    /// Custom note
    /// </summary>
    public string? CustomNote { get; set; }

    #endregion
}
