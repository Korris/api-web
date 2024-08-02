using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

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
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        #region -- Validate on server --

        if (await _context.UserReferralAvailable.AnyAsync(p => p.UserRefereeId == request.UserId))
        {
            throw new BadRequestException(E118, M118);
        }
        var userReferrerId = await _context.UserAvailable.AsNoTracking()
            .Where(p => p.ReferralCode == request.ReferralCode)
            .Select(p => p.Id)
            .FirstOrDefaultAsync();

        if (userReferrerId == Guid.Empty)
        {
            throw new BadRequestException(E116, M116);
        }

        var hasUserSocial = await _context.UserSocialAvailable.AnyAsync(p => p.Id == request.UserId, cancellationToken);
        if (!hasUserSocial)
        {
            throw new BadRequestException(E117, M117);
        }
        #endregion

        var ett = UserReferral.Create(userReferrerId, request.UserId.Value);
        await _context.UserReferrals.AddAsync(ett);
        await _context.SaveChangesAsync(default);

        //res.SetSuccess();
        return res;
    }

    #endregion
}
