using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public class BasePostLink : AuditableEntity
{
    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    public Guid PostId { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public PostLinkType Type { get; set; }
}
