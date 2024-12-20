using MediatR;
using Microsoft.EntityFrameworkCore;
using OtpNet;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserAuthenticatorDeleteH : BaseH, IRequestHandler<UserAuthenticatorDeleteR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="aes">Security aes</param>
    public UserAuthenticatorDeleteH(IMcsgContext context, ISecurityAes aes) : base(context)
    {
        _aes = aes;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserAuthenticatorDeleteR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserAuthenticatorDeleteV().Validate(request);
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
        var ett = await _context.UserAuthenticators.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(userId).ToCamelCase(), Value = userId } };
            return res.SetError(nameof(E002), E002, t);
        }

        if (!ett.IsActive)
        {
            return res.SetError(nameof(E005), E005);
        }
        #endregion

        var secretKey = _aes.DecryptText(ett.SecretKey);
        var secretKeyBytes = Base32Encoding.ToBytes(secretKey);
        var otpGenerator = new Totp(secretKeyBytes);

        var isMatch = otpGenerator.VerifyTotp(request.OtpCode, out long timeStepMatched);
        if (isMatch)
        {
            var ettRecovery = await _context.Available<UserRecovery>().Where(p => p.UserId == userId).ToListAsync(cancellationToken);
            foreach (var i in ettRecovery)
            {
                i.Delete(userId);
            }

            ett.Delete(userId);
        }
        else
        {
            return res.SetError(nameof(E301), E301);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return res.SetSuccess(isMatch);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
