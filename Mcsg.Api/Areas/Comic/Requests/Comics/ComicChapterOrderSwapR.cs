namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class ComicChapterOrderSwapR : BaseR
{
    public float Order1 { get; set; }
    public float Order2 { get; set; }
}
