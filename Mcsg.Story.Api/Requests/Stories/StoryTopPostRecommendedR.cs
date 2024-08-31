using System.ComponentModel;

namespace Mcsg.Story.Api.Requests;

public class StoryTopPostRecommendedR : StoryTopPostR
{
    [DefaultValue(3)]
    public int PageSize { get; set; }
}
