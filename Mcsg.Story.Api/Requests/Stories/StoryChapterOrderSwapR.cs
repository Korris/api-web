namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryChapterOrderSwapR : BaseR
{
    public float Order1 { get; set; }
    public float Order2 { get; set; }
}
