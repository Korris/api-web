using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Domain;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Models;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class TagPopularForPostsH : BaseH, IRequestHandler<TagPopularForPostsR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    public TagPopularForPostsH(IMcsgContext context) : base(context)
    {
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(TagPopularForPostsR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        IEnumerable<PopularTagResponse> data = [];

        var schema = "social";
        var fn = "social.fw_popular_tags_for_posts";
        var @params = "@Hide, @PostStatus, @Amount";
        var paramValues = new
        {
            Hide = request.Hides,
            PostStatus = StatusUtils.PostStatusInt,
            request.Amount
        };

        using (var connection = _context.Database.GetDbConnection())
        {
            data = await connection.QueryAsync<PopularTagResponse>(fn.ToFn("social", schema, @params), paramValues);
        }

        return res.SetSuccess(data);
    }

    #endregion
}
