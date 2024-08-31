namespace Mcsg.Story.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class StoryPostUpdateR : StoryPostFormBaseR
{
    public bool IsCompleted { get; set; }
}
