using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities
{
    using Common;

    [Table("SystemSettings")]
    public class SystemSetting : AuditableEntity
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public bool IsActive { get; set; }

        public SystemSetting()
        {
            IsActive = true;
        }
    }
}