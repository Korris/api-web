using System.ComponentModel;

namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class ComicChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
