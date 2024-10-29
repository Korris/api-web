using System.ComponentModel;

namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class DocumentChapterListR : PaginatedR
{
    [DefaultValue("Order")]
    public string? OrderBy { get; set; }
}
