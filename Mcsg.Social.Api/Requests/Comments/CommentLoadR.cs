using System.ComponentModel;

namespace Mcsg.Social.Api.Requests;

using Lib.Common.Models;

public class CommentLoadR : PaginatedR
{
    public Guid PostId { get; set; }

    [DefaultValue("LastModifiedDate")]
    public new string? OrderBy { get; set; } = "LastModifiedDate";
}
