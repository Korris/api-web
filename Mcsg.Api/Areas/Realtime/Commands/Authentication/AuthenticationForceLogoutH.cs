using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Api.Areas.Realtime.Commands;

using Common.Constants;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Realtime.Hubs;
using Mcsg.Api.Areas.Realtime.Requests;
using Mcsg.Api.Areas.Realtime.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class AuthenticationForceLogoutH : BaseH, IRequestHandler<AuthenticationForceLogoutR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="hubcontext">Hub context</param>
    public AuthenticationForceLogoutH(IMcsgContext context, IHubContext<NotificationHub> hubcontext) : base(context)
    {
        _hubContext = hubcontext;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(AuthenticationForceLogoutR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new AuthenticationForceLogoutV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var data = new { SessionId = request.IsAdministrator ? string.Empty : request.SessionId.ToString() };

        var userId = request.IsAdministrator ? request.Id.ToString() + "" : request.UserId.ToString() + "";
        await _hubContext.Clients.Group(userId).SendAsync(RealTimeTopic.ReceiveForceLogout, JsonConvert.SerializeObject(data), cancellationToken);

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
