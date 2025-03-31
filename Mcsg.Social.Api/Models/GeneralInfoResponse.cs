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

public class InteractionSummary
{
    public int CurrentCommentCount { get; set; }
    public int PreviousCommentCount { get; set; }
    public int CurrentReactionCount { get; set; }
    public int PreviousReactionCount { get; set; }
    public int CurrentShareCount { get; set; }
    public int PreviousShareCount { get; set; }
    public int CurrentTotal => CurrentCommentCount + CurrentReactionCount + CurrentShareCount;
    public int PreviousTotal => PreviousCommentCount + PreviousReactionCount + PreviousShareCount;
}
