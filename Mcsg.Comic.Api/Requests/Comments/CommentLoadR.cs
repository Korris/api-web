using System.ComponentModel;

namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class CommentLoadR : PaginatedR
{
    public Guid PostId { get; set; }

    [DefaultValue("ModifiedDate")]
    public new string? OrderBy { get; set; } = "ModifiedDate";
}
