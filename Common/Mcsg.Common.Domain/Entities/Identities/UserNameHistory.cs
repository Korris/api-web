namespace Mcsg.Common.Domain.Entities;

using Common;

public partial class UserNameHistory : AuditableEntity
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
}
