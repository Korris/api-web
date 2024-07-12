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

namespace Mcsg.Common.Core.Extensions;

using Enums;

/// <summary>
/// PostLinkType extension for using [this PostLinkType] only
/// </summary>
public static class PostLinkTypeExtension
{
    #region -- Methods --

    /// <summary>
    /// To display
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotSupportedException">NotSupportedException</exception>
    public static string ToDisplay(this PostLinkType type)
    {
        return type switch
        {
            PostLinkType.Youtube => "Youtube",
            PostLinkType.Vimeo => "Vimeo",
            PostLinkType.Instagram => "Instagram",
            PostLinkType.Facebook => "Facebook",
            PostLinkType.Twitter => "Twitter",
            PostLinkType.Video => "Video",
            PostLinkType.Other => "",
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }

    /// <summary>
    /// To ResourceType
    /// </summary>
    /// <param name="type">Type</param>
    /// <returns>Return the result</returns>
    /// <exception cref="NotSupportedException">NotSupportedException</exception>
    public static ResourceType ToResourceType(this PostLinkType type)
    {
        return type switch
        {
            PostLinkType.Youtube => ResourceType.Youtube,
            PostLinkType.Vimeo => ResourceType.Vimeo,
            PostLinkType.Instagram => ResourceType.Other,
            PostLinkType.Facebook => ResourceType.Other,
            PostLinkType.Twitter => ResourceType.Other,
            PostLinkType.Video => ResourceType.Video,
            PostLinkType.Other => ResourceType.Other,
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }

    #endregion
}
