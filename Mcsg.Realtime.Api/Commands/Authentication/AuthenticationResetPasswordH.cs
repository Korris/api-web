using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Commands;

using Common.Constants;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Responses;
using Hubs;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class AuthenticationResetPasswordH : BaseSettingH, IRequestHandler<AuthenticationResetPasswordR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="hubcontext">Hub context</param>
    public AuthenticationResetPasswordH(IMcsgContext context, ISetting setting, IHubContext<NotificationHub> hubcontext) : base(context, setting)
    {
        _hubContext = hubcontext;
        _aes = new SecurityAes(_setting.EncryptKey);
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(AuthenticationResetPasswordR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new AuthenticationResetPasswordV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var user = await GetUserByEmailOrPhone(request.Email, request.Phone, false);
        if (user == null)
        {
            return res.SetError(nameof(E303), E303);
        }

        var data = new { request.SessionId };

        await _hubContext.Clients.Group(user.Id.ToString()).SendAsync(RealTimeTopic.ReceiveForceLogout, JsonConvert.SerializeObject(data), cancellationToken);

        return res.SetSuccess(data);
    }

    private async Task<User?> GetUserByEmailOrPhone(string? email, string? phone, bool forRegister)
    {
        var encryptedEmail = _aes.EncryptText(email);
        var encryptedPhone = _aes.EncryptText(phone);

        var qUser = _context.UserAvailable;

        User? user = null;
        if (forRegister)
        {
            var qUserNameHistory = _context.UserNameHistoryAvailable.AsNoTracking();

            // Find by UserName
            user = await (from a in qUser
                          join b in qUserNameHistory on a.Id equals b.UserId
                          where !string.IsNullOrEmpty(b.UserName) && (b.UserName == encryptedEmail || b.UserName == email)
                          select a
                          ).FirstOrDefaultAsync();
        }

        // Find by Email, UserName or PhoneNumber
        if (user == null)
        {
            user = await qUser.FirstOrDefaultAsync(p => (!string.IsNullOrEmpty(p.Email) && (p.Email == encryptedEmail || p.UserName == email || p.Email == email))
                || (!string.IsNullOrEmpty(p.PhoneNumber) && (p.PhoneNumber == encryptedPhone || p.PhoneNumber == phone)));
        }

        return user;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Hub context
    /// </summary>
    private readonly IHubContext<NotificationHub> _hubContext;

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
