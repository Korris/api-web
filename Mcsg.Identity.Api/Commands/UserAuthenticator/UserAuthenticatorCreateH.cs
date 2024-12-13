using MediatR;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Responses;
using Interfaces;
using Validators;
using static Common.Domain.Entities.UserAuthenticator;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserAuthenticatorCreateH : BaseSettingH, IRequestHandler<UserAuthenticatorCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="aes">Security aes</param>
    public UserAuthenticatorCreateH(IMcsgContext context, ISetting setting, ISecurityAes aes) : base(context, setting)
    {
        _aes = aes;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserAuthenticatorCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserAuthenticatorCreateV().Validate(request);
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

        var profileName = request.ProfileName;
        var key = KeyGeneration.GenerateRandomKey(20);
        var secretKey = Base32Encoding.ToString(key);

        var encryptedSecretKey = _aes.EncryptText(secretKey) + "";
        var ett = await _context.UserAuthenticators.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            ett = UserAuthenticator.Create(encryptedSecretKey, userId);
            await _context.UserAuthenticators.AddAsync(ett, cancellationToken);
        }
        else
        {
            if (ett.IsDelete)
            {
                ett.Update(encryptedSecretKey, userId);
            }
            else
            {
                secretKey = _aes.DecryptText(ett.SecretKey) + "";
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var uri = new OtpUri(OtpType.Totp, secretKey, profileName, _setting.SiteName).ToString();
        var data = new ViewDto
        {
            QrCode = GenerateQrCodeImage(uri),
            SecretKey = secretKey
        };

        return res.SetSuccess(data);
    }

    private string GenerateQrCodeImage(string uri)
    {
        var qr = new QRCodeGenerator();
        var data = qr.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var bitmap = new BitmapByteQRCode(data);
        var bytes = bitmap.GetGraphic(20);

        return Convert.ToBase64String(bytes);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
