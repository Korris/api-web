namespace Mcsg.Social.Api.Models;

public class GeneralInfoResponse
{
    public int PostCount { get; set; }
    public int ComicCount { get; set; }
    public int StoryCount { get; set; }
    public Interactions PostInteraction { get; set; } = new Interactions();
    public Interactions ComicStoryInteraction { get; set; } = new Interactions();
    public Interactions Followers { get; set; } = new Interactions();
}

public class Interactions
{
    public int Count { get; set; }
    public double Percent { get; set; }
    public bool IsIncrease { get; set; }
}
