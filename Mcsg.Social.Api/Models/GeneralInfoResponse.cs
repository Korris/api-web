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

public class Reaction
{
    public Reaction(int last7Days, int previousLast7Days)
    {
        Last7Days = last7Days;
        PreviousLast7Days = previousLast7Days;
    }

    public int Last7Days { get; set; }
    public int PreviousLast7Days { get; set; }
}
