using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

public class PostChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
