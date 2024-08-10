namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;

    public partial class SoundService
    {
        private string GetAllSoundQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""Title""
                            , ""Url"", ""Thumbnail"", ""ArtistName""
                            , ""DurationSeconds"", ""Order""
                            FROM {_bgMediaRepository.TableName} 
                            WHERE ""IsDelete"" = false
                            ORDER BY ""Order""
                            LIMIT @PageSize
                            OFFSET @Offet ;
                            
                            SELECT COUNT(""Id"") FROM {_bgMediaRepository.TableName} 
                            WHERE ""IsDelete"" = false ;";
            }
        }
        private string SearchSoundByTitleQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""Title""
                            , ""Url"", ""Thumbnail"", ""ArtistName""
                            , ""DurationSeconds"", ""Order""
                            FROM {_bgMediaRepository.TableName} 
                            WHERE ""IsDelete"" = false AND (UPPER(""Title"") LIKE UPPER(@Keyword) OR UPPER(""ArtistName"") LIKE UPPER(@Keyword))
                            ORDER BY ""Order"" ;
                            
                            SELECT COUNT(""Id"") FROM {_bgMediaRepository.TableName} 
                            WHERE ""IsDelete"" = false AND (UPPER(""Title"") LIKE UPPER(@Keyword) OR UPPER(""ArtistName"") LIKE UPPER(@Keyword)) ;";
            }
        }
        private string GetRecentlyUseSoundQuery
        {
            get
            {
                return @$"SELECT bg.""Id"", COALESCE(bgPost.""Total"", 0) AS ""Total""
                            , bg.""Title"", bg.""Url"", bg.""Thumbnail""
                            , bg.""ArtistName"", bg.""DurationSeconds"", bg.""Order""  
                            FROM {_bgMediaRepository.TableName} bg LEFT JOIN
                            (
                                SELECT ""BackgroundMediaId"", COUNT(""Id"") AS ""Total""
                                FROM {_bgMediaPostRepository.TableName} 
                                GROUP BY ""BackgroundMediaId""
                                ORDER BY COUNT(""Id"") DESC
                            ) AS bgPost
                              ON bg.""Id"" = bgPost.""BackgroundMediaId""
                            ORDER BY COALESCE(bgPost.""Total"", 0) DESC, bg.""Order"" 
                            LIMIT @PageSize
                            OFFSET @Offet;

                            SELECT COUNT(""Id"") FROM {_bgMediaRepository.TableName} 
                            WHERE ""IsDelete"" = false ;";
            }
        }
        private string GetSoundByPostQuery
        {
            get
            {
                return @$"SELECT bgPost.""BackgroundMediaId"" AS ""Id""
                            , bg.""Title"", bg.""Url"", bg.""Thumbnail""
                            , bg.""ArtistName"", bg.""DurationSeconds"", bg.""Order""  
                            FROM {_bgMediaPostRepository.TableName} bgPost
                            LEFT JOIN {_bgMediaRepository.TableName} bg ON bg.""Id"" = bgPost.""BackgroundMediaId""
                            WHERE bgPost.""PostId"" = @PostId AND ""Status"" = {(int)BackgroundMediaPostStatus.Add}
                            ORDER BY bg.""Order"" ";
            }
        }

        private string RemoveSoundOfPostQuery
        {
            get
            {
                return @$"UPDATE {_bgMediaPostRepository.TableName}
                                SET ""Status"" = {(int)BackgroundMediaPostStatus.Remove}
                                WHERE ""PostId"" = @PostId AND ""BackgroundMediaId"" = @BackgroundMediaId";
            }
        }
    }
}
