using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public class BaseMetaData : EntityId
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? Domain { get; set; }
    public Guid? PostId { get; set; }
    public Guid? SubPostId { get; set; }
    public Guid? PostCommentId { get; set; }
    public Guid? SubPostCommentId { get; set; }
}