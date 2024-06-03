using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Services
{
	public partial class PostLinkService
	{
		private string RemoveAllLinkOfPostQuery
		{
			get
			{
				return @$"UPDATE {_postLinkRepository.TableName}
	                            SET ""IsDelete"" = true
	                            WHERE ""PostId"" = @PostId ";
			}
		}
	}
}
