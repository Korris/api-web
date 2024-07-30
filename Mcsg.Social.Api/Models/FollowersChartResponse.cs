namespace Mcsg.Social.Api.Models;

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