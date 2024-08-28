using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Filters;
using Interfaces;
using Models;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PostSearchHashTagH : BaseMinioH, IRequestHandler<PostSearchHashTagR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="businessText">Business Text</param>
    public PostSearchHashTagH(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessText) : base(context, setting, sc)
    {
        _businessText = businessText;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostSearchHashTagR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        #region -- Filter --
        string? keyword = null;

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<PostFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword;
            }
        }
        #endregion

        var pageSize = request.PageSize / 3;
        var offSetParam = (request.PageNum - 1) * pageSize;

        var qPost = "SELECT * FROM social.fn_search_hashtag(@TagName, @PostType,@PageSize, @OffSetPara)";
        var qComic = "SELECT * FROM comic.fn_search_hashtag(@TagName, @PostType, @PostStatus, @PageSize, @OffSetPara)";
        var qStory = "SELECT * FROM story.fn_search_hashtag(@TagName, @PostType, @PostStatus, @PageSize, @OffSetPara)";

        var recordComic = 0;
        var recordSocial = 0;
        var recordStory = 0;

        IEnumerable<PostSeriesTopQueryDbResponse> dataComic = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataSocial = [];
        IEnumerable<PostSeriesTopQueryDbResponse> dataStory = [];

        using (var connection = _context.Database.GetDbConnection())
        {
            if (request.Tag == "all" || request.Tag == "comic")
            {
                dataComic = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qComic, new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Comic,
                    PostStatus = (int)PostStatus.Public,
                    PageSize = pageSize,
                    OffSetPara = offSetParam
                });
                recordComic = dataComic.FirstOrDefault()?.TotalItems ?? 0;
            }

            if (request.Tag == "all" || request.Tag == "feed")
            {
                dataSocial = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qPost, new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Feed,
                    PageSize = pageSize,
                    OffSetPara = offSetParam
                });

                foreach (var item in dataSocial)
                {
                    await MappingFeedInListResponse(item);
                }

                recordSocial = dataSocial.FirstOrDefault()?.TotalItems ?? 0;
            }

            if (request.Tag == "all" || request.Tag == "story")
            {
                dataStory = await connection.QueryAsync<PostSeriesTopQueryDbResponse>(qStory, new
                {
                    TagName = keyword,
                    PostType = (int)PostType.Story,
                    PostStatus = (int)PostStatus.Public,
                    PageSize = pageSize,
                    OffSetPara = offSetParam
                });
                recordStory = dataStory.FirstOrDefault()?.TotalItems ?? 0;
            }

            var combinedItems = dataComic.Concat(dataSocial).Concat(dataStory).ToList();

            res.TotalRecords = recordStory + recordSocial + recordComic;

            var tagDataMap = new Dictionary<string, IEnumerable<PostSeriesTopQueryDbResponse>>
            {
                { "all", combinedItems },
                { "feed", dataSocial },
                { "comic", dataComic },
                { "story", dataStory }
            };

            if (tagDataMap.ContainsKey(request.Tag))
            {
                res.SetSuccess(tagDataMap[request.Tag]);
            }

        }

        await _context.Database.CloseConnectionAsync();

        return res;
    }

    /// <summary>
    /// MappingFeedInListResponse
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private async Task MappingFeedInListResponse(PostSeriesTopQueryDbResponse item)
    {
        item.Body = await _businessText.Process(item.Body);

        if (!string.IsNullOrEmpty(item.SubPostResourceStr) && item.TotalResource > 0)
        {
            var subPostResources = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostResourceStr);
            var resourceResponses = subPostResources?.Where(p => p != null).OrderBy(p => p.Order).ToList() ?? [];
            item.Resources = [];

            foreach (var resourceResponse in resourceResponses)
            {
                if (resourceResponse.Type == ResourceType.Video || resourceResponse.Type == ResourceType.Audio)
                {
                    resourceResponse.Url = await _sc.Strategy.PresignedGetObject(resourceResponse.Url, _setting.Minio.MaxExpiryInSeconds, null);
                }
                else
                {
                    resourceResponse.Url = _setting.Api.Web.Media.GetMediaPath(resourceResponse.Name, resourceResponse.Url);
                }
                item.Resources.Add(resourceResponse);
            }
        }
        else if (item.Link != null)
        {
            item.Link = new PostLinkDto
            {
                HashId = item.HashId ?? "",
                Url = item.Url ?? "",
                Type = item.Type.ToDisplay()
            };
            item.Resources =
            [
                new()
                {
                    HashId = item.HashId,
                    Url = item.Url ?? ""
                }
            ];
        }

        if (item is { MetaTitle: not null, MetaDomain: not null })
        {
            item.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDescription) ? HttpUtility.HtmlDecode(item.MetaDescription) : "",
                Domain = item.MetaDomain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaTitle) ? HttpUtility.HtmlDecode(item.MetaTitle) : "",
                Url = item.MetaUrl ?? ""
            };
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business Text
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
