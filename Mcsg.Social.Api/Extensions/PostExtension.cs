namespace Mcsg.Social.Api.Extensions
{
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
                PostLinkType.Youtube => ResourceType.YOUTUBE,
                PostLinkType.Vimeo => ResourceType.VIMEO,
                PostLinkType.Instagram => ResourceType.OTHER,
                PostLinkType.Facebook => ResourceType.OTHER,
                PostLinkType.Twitter => ResourceType.OTHER,
                PostLinkType.Video => ResourceType.VIDEO,
                PostLinkType.Other => ResourceType.OTHER,
                _ => throw new NotSupportedException($"Unsupported entity type: {type}"),
            };
        }
    }
}
