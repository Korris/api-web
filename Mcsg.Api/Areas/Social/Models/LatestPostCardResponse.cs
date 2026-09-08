using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Common.SeedWork.Enums;

/// <summary>
/// One card of the "latest posts" home block: identifiers, title and a single image.
/// Story/Comic/Document use the post thumbnail; Feed uses the first image resource of the first sub post (null when the feed has none).
/// </summary>
public class LatestPostCardResponse
{
    public Guid Id { get; set; }

    public string? HashId { get; set; }

    public PostType Type { get; set; }

    /// <summary>
    /// Post title, or the first 100 chars of the body for feeds without a title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Ready-to-use image URL, null when the post has no image
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Post creation date, or newest chapter date for series types
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Raw Dapper row for LatestPostCardResponse. Feed images are stored as object names,
/// so the query also returns BucketName/MinioInstance to build the CDN url in code.
/// </summary>
public class LatestPostCardRow : LatestPostCardResponse
{
    public string? BucketName { get; set; }

    public MinioInstanceType? MinioInstance { get; set; }
}
