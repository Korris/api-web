namespace Mcsg.Realtime.Api.Dtos;

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
    /// <param name="microService"></param>
    public ResourceCommentDto(string? userFolder, Guid postId, string hashId, string microService)
    {
        UserFolder = userFolder;
        PostId = postId;
        HashId = hashId;
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
    /// SubFolder
    /// </summary>
    public string SubFolder => $"{UserFolder}/posts/{PostId}/comments";

    #endregion
}
