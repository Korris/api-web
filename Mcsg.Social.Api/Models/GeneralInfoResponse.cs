using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

public class GeneralInfoResponse
{
    public int SocialCount { get; set; }

    [JsonIgnore]
    public int ComicCount { get; set; }

    [JsonIgnore]
    public int StoryCount { get; set; }

    public Interactions TotalComicAndStory => new()
    {
        Count = StoryCount + ComicCount,
        StoryCount = StoryCount,
        ComicCount = ComicCount
    };

    public int DocumentCount { get; set; }

    public Interactions SocialInteraction { get; set; } = new Interactions();

    public Interactions OtherInteraction { get; set; } = new Interactions();

    public Interactions Follower { get; set; } = new Interactions();
}

public class Interactions
{
    public int Count { get; set; }

    public double Percent { get; set; }

    public bool IsIncrease { get; set; }

    public int StoryCount { get; set; }

    public int ComicCount { get; set; }
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
