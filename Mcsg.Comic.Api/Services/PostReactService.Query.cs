namespace Mcsg.Comic.Api.Services
{
    public partial class PostReactService
    {
        private string GetPostReactTypeAndUsers
        {
            get
            {
                return @"SELECT ""Id"", ""ParentId"", 
				""PostId"", ""AuthorId"", ""Type"",
				""CreatedDate"", ""CreatedBy"",
				""LastModifiedDate"", ""LastModifiedBy""
					FROM {0}
					WHERE ""PostId"" = @PostId
					AND ""AuthorId"" = @UserId
					AND ""Type"" = @Type
					AND ""IsDelete"" = false";
            }
        }
    }
}
