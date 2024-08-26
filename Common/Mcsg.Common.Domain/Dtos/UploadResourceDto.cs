namespace Mcsg.Common.Domain.Dtos;

/// <summary>
/// UploadResource data transfer object
/// </summary>
public class UploadResourceDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="resourcePosts">List ResourcePost</param>
    /// <param name="userId">UserId</param>
    /// <param name="userFolder">UserFolder</param>
    /// <param name="userAvatar">UserAvatar</param>
    /// <param name="userName">UserName</param>
    public UploadResourceDto(List<ResourcePostDto> resourcePosts, Guid userId, string? userFolder, string? userAvatar, string? userName, Guid postId)
    {
        ResourcePosts = resourcePosts;
        UserId = userId;
        UserFolder = userFolder;
        UserAvatar = userAvatar;
        UserName = userName;
        PostId = postId;
    }

    /// <summary>
    /// Clone method to create a copy of the current instance
    /// </summary>
    /// <returns></returns>
    public UploadResourceDto Clone()
    {
        // Deep copy the list to avoid reference sharing
        var clonedReq = new List<ResourcePostDto>(ResourcePosts.Select(r => r.Clone()));

        // Create a new instance with the copied values
        return new UploadResourceDto(clonedReq, UserId, UserFolder, UserAvatar, UserName, PostId) { SubPostId = SubPostId };
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// List ResourcePost
    /// </summary>
    public List<ResourcePostDto> ResourcePosts { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// UserFolder
    /// </summary>
    public string? UserFolder { get; }

    /// <summary>
    /// UserAvatar
    /// </summary>
    public string? UserAvatar { get; }

    /// <summary>
    /// UserName
    /// </summary>
    public string? UserName { get; }

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; }

    /// <summary>
    /// SubPostId
    /// </summary>
    public Guid? SubPostId { get; set; }

    /// <summary>
    /// SubFolder
    /// </summary>
    public string SubFolder => $"{UserFolder}/posts/{PostId}{(SubPostId == null ? "" : $"/sub-posts/{SubPostId}")}";

    #endregion
}
