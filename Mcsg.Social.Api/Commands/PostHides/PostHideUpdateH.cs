using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class PostHideUpdateH : BaseH, IRequestHandler<PostHideUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public PostHideUpdateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostHideUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new PostHideUpdateV().Validate(request);
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

        #region -- Validate on server --
        var createdByUserId = await _context.Available<SocialPost>().Where(p => p.Id == request.PostId).Select(p => p.UserId).FirstOrDefaultAsync(cancellationToken);
        if (createdByUserId == Guid.Empty)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.PostId).ToCamelCase(), Value = request.PostId } };
            return res.SetError(nameof(E002), E002, t);
        }

        if (createdByUserId == userId)
        {
            return res.SetError(nameof(E133), E133);
        }
        #endregion

        var ett = await _context.SocialPostHides.FirstOrDefaultAsync(p => p.PostId == request.PostId && p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            ett = SocialPostHide.Create(request.PostId, userId);
            await _context.SocialPostHides.AddAsync(ett);
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
