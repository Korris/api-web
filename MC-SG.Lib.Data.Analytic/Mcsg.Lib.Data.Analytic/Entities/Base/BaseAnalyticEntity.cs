using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Analytic.Entities.Base
{
	public class BaseAnalyticEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		public BaseAnalyticEntity()
		{
			Id = Guid.NewGuid();
		}
	}
}
