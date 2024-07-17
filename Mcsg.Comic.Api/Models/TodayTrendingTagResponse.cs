namespace Mcsg.Comic.Api.Models;

public class TodayTrendingTagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public int TotalCount { get; set; }
}
