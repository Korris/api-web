using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Filters;
using Models;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PostSearchHashTagH : BaseH, IRequestHandler<PostSearchHashTagR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public PostSearchHashTagH(IMcsgContext context) : base(context) { }

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
                    OffSetPara = (int)request.Offset
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
                    OffSetPara = (int)request.Offset
                });
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
                    OffSetPara = (int)request.Offset
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

    #endregion
}
