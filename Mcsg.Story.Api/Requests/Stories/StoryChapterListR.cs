using System.ComponentModel;

namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class StoryChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
