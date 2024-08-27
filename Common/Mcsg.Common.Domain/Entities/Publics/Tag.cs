using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public class Tag : AuditableEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    public Guid? AuthorId { get; set; }
    public string? Name { get; set; }
}