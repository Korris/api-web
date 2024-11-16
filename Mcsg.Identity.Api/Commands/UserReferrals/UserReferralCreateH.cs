using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserReferralCreateH : BaseH, IRequestHandler<UserReferralCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserReferralCreateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<SingleResponse> Handle(UserReferralCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserReferralCreateV().Validate(request);
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

        if (await _context.UserReferralAvailable.AnyAsync(p => p.UserRefereeId == request.UserId))
        {
            throw new BadRequestException(nameof(E118), E118);
        }
        var userReferrerId = await _context.UserAvailable.AsNoTracking()
            .Where(p => p.ReferralCode == request.ReferralCode)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (userReferrerId == Guid.Empty)
        {
            throw new BadRequestException(nameof(E116), E116);
        }

        var hasUserSocial = await _context.UserSocialAvailable.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
        if (!hasUserSocial)
        {
            throw new BadRequestException(nameof(E117), E117);
        }
        #endregion

        var ett = UserReferral.Create(userReferrerId, request.UserId.Value);
        await _context.UserReferrals.AddAsync(ett, cancellationToken);
        await _context.SaveChangesAsync(default);

        return res.SetSuccess(ett.ToViewDto());
    }

    #endregion
}
