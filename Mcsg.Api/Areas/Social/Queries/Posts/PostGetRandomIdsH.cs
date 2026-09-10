using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Social.Queries;

using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Requests;
using Mcsg.Api.Interfaces;

/// <summary>
/// Handler for getting random post ids
/// </summary>
public class PostGetRandomIdsH : BaseSettingH, IRequestHandler<PostGetRandomIdsR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public PostGetRandomIdsH(IMcsgContext context, ISetting setting) : base(context, setting)
    {
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostGetRandomIdsR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var hasExcludeHashId = !string.IsNullOrEmpty(request.ExcludeHashId);
        var take = 1000;

        IQueryable<string> qHashId;
        if (request.IsHaveMedia)
        {
            qHashId = from p in _context.Available<SocialPost>()
                      join sp in _context.Available<SocialSubPost>() on p.Id equals sp.PostId
                      join sr in _context.Available<SocialResource>() on sp.Id equals sr.SubPostId
                      group p by p.HashId into grouped
                      select grouped.Key;
        }
        else
        {
            qHashId = _context.Available<SocialPost>().Select(p => p.HashId);
        }

        if (hasExcludeHashId)
        {
            qHashId = qHashId.Where(p => p != request.ExcludeHashId);
        }

        var randomIds = await qHashId
            .OrderBy(_ => Guid.NewGuid())
            .Take(take)
            .ToListAsync(cancellationToken);

        return res.SetSuccess(randomIds);
    }

    #endregion
}
