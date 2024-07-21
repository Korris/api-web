using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
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