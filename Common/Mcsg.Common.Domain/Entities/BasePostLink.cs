using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public class BasePostLink : AuditableEntity
{
    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? Url { get; set; }

    [StringLength(Validator.Description.Max)]
    public string? Description { get; set; }

    public Guid PostId { get; set; }
    public PostLinkType Type { get; set; }
}
