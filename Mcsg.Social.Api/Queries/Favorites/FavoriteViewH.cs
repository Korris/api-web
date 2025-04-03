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

        var comicCount = await Count<ComicPost, ComicPostFavorite, ComicSubPost>(userId, request);
        var documentCount = await Count<DocumentPost, DocumentPostFavorite, DocumentSubPost>(userId, request);
        var socialCount = await Count<SocialPost, SocialPostFavorite, SocialSubPost>(userId, request);
        var storyCount = await Count<StoryPost, StoryPostFavorite, StorySubPost>(userId, request);

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
    /// <typeparam name="SP"></typeparam>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<int> Count<P, PF, SP>(Guid userId, FavoriteViewR request) where P : BasePost where PF : BasePostFavorite where SP : BaseSubPost
    {
        var qPost = _context.Set<P>().Where(p => !p.IsDelete && p.Status == PostStatus.Public && p.Permission == PostPermission.Public);
        var qPostFavorite = _context.Set<PF>().Where(p => !p.IsDelete);
        var qSubPost = _context.Set<SP>().Where(p => !p.IsDelete);

        var q = from post in qPost
                join favorite in qPostFavorite on post.Id equals favorite.PostId
                join sp in qSubPost on post.Id equals sp.PostId into spGroup
                from sp in spGroup.DefaultIfEmpty()
                where !request.Hides.Contains((int)post.Hide)
                           && (post.Type == PostType.Feed ||
                               (sp != null && StatusUtils.PostStatuses.Contains(sp.Status)
                                && sp.Permission != PostPermission.Private && sp.PublishDate < DateTime.UtcNow))
                select favorite;

        return await q.Distinct().CountAsync(p => p.UserId == userId);
    }

    #endregion
}
