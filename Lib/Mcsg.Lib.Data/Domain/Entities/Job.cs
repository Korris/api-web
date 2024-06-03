using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("Jobs")]
    public class Job : AuditableEntity
    {
        public JobType JobType { get; set; }
        public JobCategory JobCategory { get; set; }
        public string Data { get; set; }
        public JobStatus Status { get; set; }
        public string Error { get; set; }
    }
}