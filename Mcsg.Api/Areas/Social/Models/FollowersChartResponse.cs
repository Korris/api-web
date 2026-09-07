namespace Mcsg.Api.Areas.Social.Models;

public class ChartResponse
{
    public string? Label { get; set; }
    public int Quantity { get; set; }
}

public class FollowersChartResponse
{
    public List<UserFollowedResponse> UserFollowedResponses { get; set; } = new List<UserFollowedResponse>();
    public List<ChartResponse> ChartResponse { get; set; } = new List<ChartResponse>();
    public Interactions FollowerInteractions { get; set; } = new Interactions();
}

public class PostChartResponse
{
    public List<ChartResponse> ReactionChartResponse { get; set; } = new List<ChartResponse>();
    public List<ChartResponse> CommentChartResponse { get; set; } = new List<ChartResponse>();
    public List<ChartResponse> ShareChartResponse { get; set; } = new List<ChartResponse>();
    public Interactions PostInteractions { get; set; } = new Interactions();
    public Interactions PostCommentInteractions { get; set; } = new Interactions();
    public Interactions PostReactionInteractions { get; set; } = new Interactions();
    public Interactions PostShareInteractions { get; set; } = new Interactions();
}

public class FeedChartResponse
{
    public List<ChartResponse> ChartResponseComment { get; set; } = new List<ChartResponse>();
    public List<ChartResponse> ChartResponseReact { get; set; } = new List<ChartResponse>();
    public List<ChartResponse> ChartResponseShare { get; set; } = new List<ChartResponse>();
    public Interactions ReactionInteractions { get; set; } = new Interactions();
    public Interactions CommentInteractions { get; set; } = new Interactions();
    public Interactions ShareInteractions { get; set; } = new Interactions();
    public Interactions PostInteractions { get; set; } = new Interactions();
}
