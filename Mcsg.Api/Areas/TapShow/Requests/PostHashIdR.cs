namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Children of a post (chapters, characters); PostHashId from the route
/// </summary>
public class PostHashIdR : BaseR
{
    public string? PostHashId { get; set; }
}
