namespace Mcsg.Api.Areas.Social.Dtos;

/// <summary>
/// Row shape of the api-mobile DB function social.fm_posts_by_username (used by PATCH v1/LoadFeed).
/// That function returns the mobile column names (totalresources, subpoststring, subpostresourcestring),
/// while FeedService.MappingFeedInListRespone reads the web names (TotalResource, SubPostStr, SubPostResourceStr).
/// The extra properties below let Dapper bind the mobile columns and forward them to the web fields.
/// </summary>
public class FeedsByUserNameQueryDbDto : FeedsListQueryDbDto
{
    /// <summary>
    /// fm_ column "totalresources" (int8): number of sub-posts of the post
    /// </summary>
    public long TotalResources
    {
        get => TotalResource;
        set => TotalResource = (int)value;
    }

    /// <summary>
    /// fm_ column "subpoststring" (jsonb): sub-posts of the post
    /// </summary>
    public string? SubPostString
    {
        get => SubPostStr;
        set => SubPostStr = value;
    }

    /// <summary>
    /// fm_ column "subpostresourcestring" (jsonb): first resource of each sub-post
    /// </summary>
    public string? SubPostResourceString
    {
        get => SubPostResourceStr;
        set => SubPostResourceStr = value;
    }
}
