#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Commands;

using Api.Validators;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Interfaces;
using Mcsg.Chat.Api.Protos;
using Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserSyncToChatH : BaseSettingH, IRequestHandler<UserSyncToChatR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public UserSyncToChatH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserSyncToChatR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserSyncToChatV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var rsp = await SyncListUserToChat();
        return res.SetSuccess(rsp);
    }

    private async Task<SyncUserRsp> SyncListUserToChat()
    {
        var res = new SyncUserRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Chat.Chat!);
            var client = new UserSyncProto.UserSyncProtoClient(channel);

            var etts = await _context.Users
                .OrderBy(p => p.CreatedOn)
                .Select(p => new SyncUserProtoDto
                {
                    UserId = p.Id.ToString(),
                    ProfileName = p.ProfileName,
                    Avatar = p.Avatar ?? "",
                    UserName = p.UserName
                }).ToListAsync();

            var request = new SyncUserReq();
            request.Users.AddRange(etts);
            var rsp = await client.SyncUsersAsync(request);
            res.Message = rsp.Message;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    #endregion
}
