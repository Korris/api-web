#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

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
