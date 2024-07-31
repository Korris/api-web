namespace Mcsg.Social.Api.Interfaces;

using Models;

public interface IChartService
{
    Task<GeneralInfoResponse> GetGeneralInfo(Guid? userId);
    Task<FollowersChartResponse> GetFollowersChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days);
    Task<ComicChartResponse> GetComicOrStoryChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days, bool isComic);
    Task<FeedChartResponse> GetInteractionChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days);
}
