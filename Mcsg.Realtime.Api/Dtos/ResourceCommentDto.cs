namespace Mcsg.Realtime.Api.Dtos;

using Common.Core.Enums;
using Constants;

/// <summary>
/// ResourceCommentDto
/// </summary>
public class ResourceCommentDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="userFolder"></param>
    /// <param name="postId"></param>
    /// <param name="hashId"></param>
    /// <param name="type"></param>
    /// <param name="microService"></param>
    public ResourceCommentDto(string? userFolder, Guid postId, string hashId, string type, string microService)
    {
        UserFolder = userFolder;
        PostId = postId;
        HashId = hashId;
        _type = type;
        MicroService = microService;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// UserFolder
    /// </summary>
    public string? UserFolder { get; }

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; }

    /// <summary>
    /// HashId
    /// </summary>
    public string HashId { get; }

    /// <summary>
    /// Micro service
    /// </summary>
    public string MicroService { get; }

    /// <summary>
    /// Location type
    /// </summary>
    public ResourceLocationType LocationType => _type == PostTypes.Post ? ResourceLocationType.PostComment : ResourceLocationType.SubPostComment;

    #endregion

    #region -- Fields --

    /// <summary>
    /// Type
    /// </summary>
    private string _type;

    #endregion
}
