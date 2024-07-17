using System.ComponentModel;

namespace Mcsg.Comic.Api.Requests;

public class ComicChapterListR : BasePageResultR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
