namespace Mcsg.Function.Job.Services
{
    using Common.Core.Enums;

    public partial class SyncDataService
    {
        private string GetAllUserSession
        {
            get
            {
                return @"SELECT ""Id"", ""LoginProvider"", ""LoginDateUtc"", ""ExpiredDateUtc"", ""UserName"", ""Email"", 
                            ""LastName"", ""FirstName"", ""UserId"", ""Roles"", ""Claims"", 
                            ""LastActionDateUtc"", ""CreatedOn"", ""CreatedBy"", ""ModifiedOn"", ""ModifiedBy"", ""IsDelete"", ""ProfileName"", ""ProfileId"", ""UserAvatar"", ""PremiumDate""
	                            FROM public.""Sessions""
	                            WHERE ""UserId"" = @UserId AND ""ExpiredDateUtc"" > @DateNow
	                            ORDER BY ""CreatedOn"" DESC";
            }
        }

        private string UpdatePremiumDate
        {
            get
            {
                return @"UPDATE ""Sessions""
                                    SET ""PremiumDate"" = @PremiumDate
                                    WHERE ""UserId"" = @UserId AND ""ExpiredDateUtc"" > @DateTimeNow;
                        UPDATE identity.""Users""
                                    SET ""PremiumDate"" = @PremiumDate
                                    WHERE ""Id"" = @UserId;";
            }
        }

        #region Buy chapter
        private string GetUserExclusiveSubPosts
        {
            get
            {
                return @"SELECT ""Id"",""CreatedOn""
	            FROM public.""UserExclusiveSubPosts""
	            WHERE ""UserId"" = @UserId AND ""SubPostId"" = @SubPostId AND ""IsDelete"" = false";
            }
        }
        #endregion

        #region Buy series
        private string GetSeriesChaptersByPostId
        {
            get
            {
                return $@"SELECT ""Id"", ""Title"", ""PublishDate"", ""Permission"", ""Order""
					FROM {_subPostRepository.TableName} 
					WHERE ""PostId"" = @PostId 
                            AND ""IsDelete"" = false 
                            AND ""Permission"" <> {(int)PostPermission.Private}
					ORDER BY ""Order"" ; ";
            }
        }
        private string GetUserExclusiveSubPostsList
        {
            get
            {
                return @"SELECT ""Id"",""CreatedOn""
	            FROM public.""UserExclusiveSubPosts""
	            WHERE ""UserId"" = @UserId AND ""SubPostId"" = ANY(@SubPostIds) AND ""IsDelete"" = false";
            }
        }
        #endregion
    }
}
