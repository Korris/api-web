namespace Mcsg.Social.Api.Extensions;

using Common.Core.Enums;
using Constants;
using Enums;
using Lib.Data.Enums;
using Models;

public static class PostExtension
{
    public static string ToPostSeriesStatus(this PostSeriesSelectedType type)
    {
        return type switch
        {
            PostSeriesSelectedType.HIT => PostConst.PostSeriesStatus.Hit,
            PostSeriesSelectedType.LATEST => PostConst.PostSeriesStatus.Latest,
            PostSeriesSelectedType.COMPLETED => PostConst.PostSeriesStatus.Completed,
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }
    public static string ToSeriesStatus(this PostSeriesResponse model)
    {
        return model.IsCompleted switch
        {
            false => PostConst.PostSeriesStatus.Latest,
            true => PostConst.PostSeriesStatus.Completed,
            _ => PostConst.PostSeriesStatus.Latest,
        };
    }
    public static string ToDisplay(this PostLinkType type)
    {
        return type switch
        {
            PostLinkType.Youtube => "Youtube",
            PostLinkType.Vimeo => "Vimeo",
            PostLinkType.Instagram => "Instagram",
            PostLinkType.Facebook => "Facebook",
            PostLinkType.Twitter => "Twitter",
            PostLinkType.Video => "Video",
            PostLinkType.Other => "",
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }
    public static ResourceType ToResourceType(this PostLinkType type)
    {
        return type switch
        {
            PostLinkType.Youtube => ResourceType.Youtube,
            PostLinkType.Vimeo => ResourceType.Vimeo,
            PostLinkType.Instagram => ResourceType.Other,
            PostLinkType.Facebook => ResourceType.Other,
            PostLinkType.Twitter => ResourceType.Other,
            PostLinkType.Video => ResourceType.Video,
            PostLinkType.Other => ResourceType.Other,
            _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
        };
    }
}
