using System.ComponentModel;

namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class CommentLoadR : PaginatedR
{
    public Guid PostId { get; set; }

    [DefaultValue("CreatedOn")]
    public new string? OrderBy { get; set; } = "CreatedOn";
}
