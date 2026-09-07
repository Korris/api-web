namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentChapterOrderSwapR : BaseR
{
    public float Order1 { get; set; }
    public float Order2 { get; set; }
}
