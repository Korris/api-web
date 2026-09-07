namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Extensions;

/// <summary>
/// Request
/// </summary>
public class PostSearchHashTagR : PagingR
{
    #region -- Properties --

    /// <summary>
    /// Tag
    /// </summary>
    public string Tag { get; set; } = default!;

    /// <summary>
    /// Type
    /// </summary>
    public PostType Type => Tag.ToEnum(PostType.All);

    #endregion
}
