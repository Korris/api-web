namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Models;

public interface IChartService
{
    /// <summary>
    /// Chart information for User
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<GeneralInfoResponse> GetGeneralInfo(Guid? userId);

    Task<FollowersChartResponse> GetFollowersChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days);

    /// <summary>
    /// Chart information for Comic, Document and Story
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="timezoneOffset"></param>
    /// <param name="isGetDataIn7Days"></param>
    /// <param name="postType"></param>
    /// <returns></returns>
    Task<PostChartResponse> GetChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days, PostType postType);

    Task<FeedChartResponse> GetInteractionChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days);
}
