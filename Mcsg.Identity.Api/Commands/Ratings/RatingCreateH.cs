using MediatR;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class RatingCreateH : BaseH, IRequestHandler<RatingCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public RatingCreateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(RatingCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new RatingCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        // Create
        var satisfaction = request.Satisfaction.ToEnum(SatisfactionLevel.Neutral);
        var ett = Rating.Create(satisfaction, request.UserId, request.Email!, request.Comment + ""!);
        await _context.Ratings.AddAsync(ett, cancellationToken);
        await _context.SaveChangesAsync(default);

        return res.SetSuccess(ett.ToViewDto());
    }

    #endregion

}