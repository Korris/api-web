namespace Mcsg.Lib.Data.Domain.Entities;

using Lib.Data.Domain.Entities.Common;

public partial class UserNameHistory : AuditableEntity
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
}
