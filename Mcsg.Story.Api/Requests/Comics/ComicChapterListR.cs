using System.ComponentModel;

namespace Mcsg.Story.Api.Requests;

public class ComicChapterListR : BasePageResultR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
