using System.Text.Json.Serialization;

namespace Mcsg.Common.Domain.Dtos;

using SeedWork.Converters;
using SeedWork.Enums;

/// <summary>
/// Latest comment attached to a feed item (shared by Comic/Story/Document/Social feeds)
/// </summary>
public class CommentLateDto
{
    #region -- Properties --

    /// <summary>
    /// Id
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// HashId
    /// </summary>
    public string? HashId { get; set; }

    /// <summary>
    /// Body
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// AuthorId
    /// </summary>
    public Guid? AuthorId { get; set; }

    /// <summary>
    /// AuthorName
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// CreatedOn
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Avatar
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// GifId
    /// </summary>
    public string? GifId { get; set; }

    /// <summary>
    /// ResourceId
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// ResourceUrl (object name, resolved to public url by handler)
    /// </summary>
    public string? ResourceUrl { get; set; }

    /// <summary>
    /// MinioInstance
    /// </summary>
    public MinioInstanceType? MinioInstance { get; set; }

    /// <summary>
    /// BucketName
    /// </summary>
    public string? BucketName { get; set; }

    /// <summary>
    /// CustomNote
    /// </summary>
    public string? CustomNote { get; set; }

    #endregion
}
