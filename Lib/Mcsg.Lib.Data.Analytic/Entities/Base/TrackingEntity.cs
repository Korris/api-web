using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Analytic.Entities.Base
{
	public class TrackingEntity:BaseAnalyticEntity
    {
		public DateTime CreatedDate { get; set; }
		public Guid? CreatedBy { get; set; }

		public TrackingEntity()
		{
			CreatedDate = DateTime.UtcNow;
		}
	}
}
