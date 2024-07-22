namespace Mcsg.Social.Api.Requests;

using Common.Core.Enums;

public class StoryChapterPostR
{
    public string? Title { get; set; }
    public string? Name { get; set; }

    #region Setting
    public bool IsPublicNow { get; set; }
    public PostPermission Permission { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public bool IsEnableComment { get; set; }
    #endregion
}
