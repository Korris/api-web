namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class ChapterOrderR : BaseR
{
    public string? HashId { get; set; }
    public float? Order { get; set; }
}
