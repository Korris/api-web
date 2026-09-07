using System.ComponentModel;

namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class PostChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
