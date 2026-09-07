using MediatR;
using Microsoft.EntityFrameworkCore;
using OtpNet;

namespace Mcsg.Api.Areas.Identity.Commands;
using Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Identity.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserAuthenticatorUpdateH : BaseH, IRequestHandler<UserAuthenticatorUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="aes">Security aes</param>
    public UserAuthenticatorUpdateH(IMcsgContext context, ISecurityAes aes) : base(context)
    {
        _aes = aes;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserAuthenticatorUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserAuthenticatorUpdateV().Validate(request);
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
        #endregion

        var isMatch = false;
        var otp = request.OtpCode;
        List<string> codes = [];
        if (otp != null) // active 2FA
        {
            if (ett.IsActive)
            {
                return res.SetError(nameof(E004), E004);
            }

            var secretKey = _aes.DecryptText(ett.SecretKey);
            var secretKeyBytes = Base32Encoding.ToBytes(secretKey);
            var otpGenerator = new Totp(secretKeyBytes);

            isMatch = otpGenerator.VerifyTotp(otp, out long timeStepMatched);
            if (isMatch)
            {
                var ettRecovery = new List<UserRecovery>();

                codes = GenerateRecoveryCodes();
                foreach (var i in codes)
                {
                    var encryptedCode = _aes.EncryptText(i) + "";
                    ettRecovery.Add(UserRecovery.Create(encryptedCode, userId));
                }

                await _context.UserRecoveries.AddRangeAsync(ettRecovery, cancellationToken);

                ett.Update(userId);
            }
            else
            {
                return res.SetError(nameof(E301), E301);
            }
        }
        else // enable/disable for Login or Transaction
        {
            if (!ett.IsActive)
            {
                return res.SetError(nameof(E004), E004);
            }

            ett.Update(request.IsLogin, request.IsTransaction, userId);
            isMatch = true;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var data = new
        {
            isMatch,
            recoveryCodes = string.Join(",", codes)
        };

        return res.SetSuccess(data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
