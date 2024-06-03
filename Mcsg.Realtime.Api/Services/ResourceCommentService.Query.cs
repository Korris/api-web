namespace Mcsg.Realtime.Api.Services
{
    public partial class ResourceCommentService
    {
        private string GetResourceQuery
        {
            get
            {
                // Select all for update behavior
                return @$"SELECT ""Id"", ""HashId"", ""Title"", ""Name""
                                    , ""Type"", ""AuthorId"", ""LocationType"", ""Status""
                                    ,""Size"", ""Url"", ""SubPostId""
                        FROM {_resourceRepository.TableName}
                     WHERE ""HashId"" = @HashId AND ""IsDelete"" = false ";
            }
        }
    }
}
