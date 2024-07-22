namespace Mcsg.Story.Api.Services
{
    public partial class PostReactService
    {
        private string GetPostReactTypeAndUsers
        {
            get
            {
                return @"SELECT ""Id"", ""ParentId"", 
				""PostId"", ""AuthorId"", ""Type"",
				""CreatedOn"", ""CreatedBy"",
				""ModifiedOn"", ""ModifiedBy""
					FROM {0}
					WHERE ""PostId"" = @PostId
					AND ""AuthorId"" = @UserId
					AND ""Type"" = @Type
					AND ""IsDelete"" = false";
            }
        }
    }
}
