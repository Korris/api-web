using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class PostFavoriteUpdateH : BaseH, IRequestHandler<PostFavoriteUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public PostFavoriteUpdateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<SingleResponse> Handle(PostFavoriteUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new PostFavoriteUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }

        #region -- Validate on server --
        var hasPost = await _context.SocialPostAvailable.AnyAsync(p => p.Id == request.PostId, cancellationToken);
        if (!hasPost)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.PostId).ToCamelCase(), Value = request.PostId } };
            return res.SetError(nameof(E002), E002, t);
        }
        #endregion

        var userId = request.UserId.Value;

        var ett = await _context.SocialPostFavorites.FirstOrDefaultAsync(p => p.PostId == request.PostId && p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            ett = SocialPostFavorite.Create(request.PostId, userId);
            await _context.SocialPostFavorites.AddAsync(ett);
        }
        else
        {
            ett.Update(userId);
        }

        await _context.SaveChangesAsync(default);

        return res.SetSuccess(ett.ToViewDto());
    }

    #endregion
}
