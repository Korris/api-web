namespace Mcsg.Social.Api.Interfaces;
using Models;

public interface IChartService
{
    Task<GeneralInfoResponse> GetGeneralInfo(Guid? userId);
    Task<FollowersChartResponse> GetFollowersChartInfo(Guid? userId, bool isGetDataIn7Days);
    Task<ComicChartResponse> GetComicOrStoryChartInfo(Guid? userId, bool isGetDataIn7Days, bool isComic = false);
    Task<FeedChartResponse> GetInteractionChartInfo(Guid? userId, bool isGetDataIn7Days);
}
