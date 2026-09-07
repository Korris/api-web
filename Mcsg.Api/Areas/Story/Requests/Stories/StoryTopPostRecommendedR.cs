using System.ComponentModel;

namespace Mcsg.Api.Areas.Story.Requests;

public class StoryTopPostRecommendedR : StoryTopPostR
{
    [DefaultValue(3)]
    public int PageSize { get; set; }
}
