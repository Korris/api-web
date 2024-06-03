using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Analytic.Entities.Base
{
	public class AuditableEntity: TrackingEntity
    {
		public DateTime ModifiedDate { get; set; }
		public Guid? ModifiedBy { get; set; }
		public bool IsDelete { get; set; }

		public AuditableEntity()
		{
			ModifiedDate = DateTime.UtcNow;
		}
	}
}
