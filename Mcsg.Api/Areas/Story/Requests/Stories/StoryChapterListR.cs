using System.ComponentModel;

namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class StoryChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
