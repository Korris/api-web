using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public class BaseMetaData : EntityId
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? Url { get; set; }

    [StringLength(Validator.Description.Max)]
    public string? Description { get; set; }

    [StringLength(32)]
    public string? Domain { get; set; }

    public Guid? PostId { get; set; }
    public Guid? SubPostId { get; set; }
    public Guid? PostCommentId { get; set; }
    public Guid? SubPostCommentId { get; set; }
}