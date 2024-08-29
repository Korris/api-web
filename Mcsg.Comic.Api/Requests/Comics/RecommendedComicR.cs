namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class RecommendedComicR : PaginatedR
{
    public int Number { get; set; }
}
