namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class SystemSetting : AuditableEntity
{
    public string? Key { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }

    public SystemSetting()
    {
        IsActive = true;
    }
}