namespace Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// Request
/// </summary>
public class StoryPostUpdateR : StoryPostFormBaseR
{
    public bool IsCompleted { get; set; }
}
