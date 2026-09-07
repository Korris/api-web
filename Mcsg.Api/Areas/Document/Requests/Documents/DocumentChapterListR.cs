using System.ComponentModel;

namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
