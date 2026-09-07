using System.ComponentModel;

namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class ComicChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
