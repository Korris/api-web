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

namespace Mcsg.Api.Areas.Identity.Commands;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Requests;
using Mcsg.Api.Areas.Identity.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserSyncToAnaH : BaseSettingH, IRequestHandler<UserSyncToAnaR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public UserSyncToAnaH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserSyncToAnaR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserSyncToAnaV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var rsp = await SyncCreateToAna();
        return res.SetSuccess(rsp);
    }

    private async Task<UserCreateRsp> SyncCreateToAna()
    {
        var res = new UserCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new UserProto.UserProtoClient(channel);

            var etts = await _context.Users.Select(p => new UserProtoDto
            {
                UserId = p.Id.ToString(),
                Username = p.UserName,
                UserStatus = (int)p.Status,
                ProfileId = p.ProfileId,
                CreatedOn = p.CreatedOn.ToString(),
                CreatedBy = (p.CreatedBy == null ? p.Id : p.CreatedBy).ToString(),
                ModifiedOn = p.ModifiedOn == null ? null : p.ModifiedOn.ToString(),
                ModifiedBy = p.ModifiedBy == null ? null : p.ModifiedBy.ToString()
            }).OrderBy(p => p.CreatedOn).ToListAsync();

            var request = new UserCreateReq();
            request.Items.AddRange(etts);
            var rsp = await client.CreateAsync(request);

            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
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
