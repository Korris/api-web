using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class SystemConfig : AuditableEntity
{
    [StringLength(Validator.Description.Max)]
    public string? Description { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Key { get; set; }

    public string? Value { get; set; }

    [StringLength(32)]
    public string? DataType { get; set; }
}