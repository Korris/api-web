namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class ChapterOrderR : BaseR
{
    public string? HashId { get; set; }
    public float? Order { get; set; }
}
