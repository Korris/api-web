using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

public class ComicChapterListR : BasePageResultR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
