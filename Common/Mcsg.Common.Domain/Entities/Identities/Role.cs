using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using SeedWork.Constants;

public partial class Role : IdentityRole<Guid>
{
    [StringLength(Validator.Name.Max)]
    public string? DisplayName { get; set; }
}
