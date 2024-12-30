using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Commands;

using Common.Constants;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Hubs;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class AuthenticationVerifyOtpH : BaseH, IRequestHandler<AuthenticationVerifyOtpR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="hubcontext">Hub context</param>
    public AuthenticationVerifyOtpH(IMcsgContext context, IHubContext<NotificationHub> hubcontext) : base(context)
    {
        _hubContext = hubcontext;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(AuthenticationVerifyOtpR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new AuthenticationVerifyOtpV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var data = request.SessionId.ToString();

        var userId = request.UserId.ToString() + "";
        await _hubContext.Clients.Group(userId).SendAsync(RealTimeTopic.ReceiveVerifyOtp, JsonConvert.SerializeObject(data), cancellationToken);

        return res.SetSuccess(data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Hub context
    /// </summary>
    private readonly IHubContext<NotificationHub> _hubContext;

    #endregion
}
