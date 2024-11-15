using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Queries;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class FavoriteViewH : BaseH, IRequestHandler<FavoriteViewR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public FavoriteViewH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(FavoriteViewR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new FavoriteViewV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }

        var userId = request.UserId.Value;

        var comicCount = await Count<ComicPost, ComicPostFavorite>(userId);
        var documentCount = await Count<DocumentPost, DocumentPostFavorite>(userId);
        var socialCount = await Count<SocialPost, SocialPostFavorite>(userId);
        var storyCount = await Count<StoryPost, StoryPostFavorite>(userId);

        var data = new
        {
            comicCount,
            documentCount,
            socialCount,
            storyCount
        };

        return res.SetSuccess(data);
    }

    /// <summary>
    /// Count
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="PF"></typeparam>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<int> Count<P, PF>(Guid userId) where P : BasePost where PF : BasePostFavorite
    {
        var qPost = _context.Set<P>().Where(p => !p.IsDelete && p.Status == PostStatus.Public);
        var qPostFavorite = _context.Set<PF>().Where(p => !p.IsDelete);

        var q = from post in qPost
                join favorite in qPostFavorite
                on post.Id equals favorite.PostId
                select favorite;

        return await q.CountAsync(p => p.UserId == userId);
    }

    #endregion
}
