using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class SubPostsDeleteH : BaseSettingH, IRequestHandler<SubPostDeleteR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public SubPostsDeleteH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(SubPostDeleteR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new SubPostDeleteV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            res.SetError(E000, M000, t);
            return res;
        }

        if (request.UserId == null)
        {
            res.SetError(E109, M109);
            return res;
        }

        var userId = request.UserId.Value;

        #region -- Validate on server --
        // SubPost
        var ett = await _context.SocialSubPostAvailable.FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.Id).ToCamelCase(), Value = request.Id } };
            res.SetError(E002, M002, t);
            return res;
        }
        #endregion

        if (ett.IsDelete)
        {
            res.SetError(E003, M003);
            return res;
        }

        // Delete
        ett.Delete(userId);
        await _context.SaveChangesAsync(cancellationToken);

        _ = Task.Run(async () => await SyncDeleteToAna(ett.Id));

        return res;
    }

    private async Task<SocialSubDeleteRsp> SyncDeleteToAna(Guid id)
    {
        var res = new SocialSubDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new SocialSubProto.SocialSubProtoClient(channel);
            var request = new SocialSubDeleteReq
            {
                Items =
                {
                    new SocialSubDeleteDto {Id = id.ToString()}
                }
            };

            var rsp = await client.DeleteAsync(request);
            res.Message = rsp.Message;
            res.Items.Add(rsp.Items);
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
