using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;

namespace Mcsg.Api.Services
{
    public partial class FileService
    {
        private string GetListResourceQuery
        {
            get
            {
                // Select all for update behavior
                return @$"SELECT * FROM {_resourceRepository.TableName}
                     WHERE ""HashId"" = ANY(@HashIds)";
            }
        }
		private string GetResourcesByPostIdQuery
		{
			get
			{
				// Select all for update behavior
				return @$"SELECT * FROM {_resourceRepository.TableName}
                     WHERE  ""Type"" <> '3' 
                                AND ""IsDelete"" = false 
                                AND ""SubPostId"" IN (SELECT ""Id"" FROM {_subPostRepository.TableName} WHERE ""PostId"" = @PostId)";
			}
		}
		private string RemoveFilesOfPostQuery
		{
			get
			{
				return @$"UPDATE {_resourceRepository.TableName}
	                            SET ""IsDelete"" = true
	                            WHERE ""HashId"" = ANY(@HashIds) ;
						UPDATE {_subPostRepository.TableName}
	                            SET ""IsDelete"" = true
	                            WHERE ""Id"" = ANY(@SubPostIds) ;";
			}
		}
	}
}
