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

namespace Mcsg.Api.Areas.Story.Commands;
using Mcsg.Api.Interfaces;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Requests;
using Mcsg.Api.Areas.Story.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class SubPostSyncToAnaH : BaseSettingH, IRequestHandler<SubPostSyncToAnaR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public SubPostSyncToAnaH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(SubPostSyncToAnaR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new SubPostSyncToAnaV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var rsp = await SyncCreateToAna();
        return res.SetSuccess(rsp);
    }

    private async Task<StorySubCreateRsp> SyncCreateToAna()
    {
        var res = new StorySubCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new StorySubProto.StorySubProtoClient(channel);

            var etts = await _context.StorySubPosts.Select(p => new StorySubProtoDto
            {
                PostId = p.PostId.ToString(),
                SubPostId = p.Id.ToString(),
                UserId = p.UserId.ToString(),
                CreatedOn = p.CreatedOn.ToString(),
                CreatedBy = p.CreatedBy == null ? null : p.CreatedBy.ToString(),
                ModifiedOn = p.ModifiedOn == null ? null : p.ModifiedOn.ToString(),
                ModifiedBy = p.ModifiedBy == null ? null : p.ModifiedBy.ToString(),
                IsPremium = p.IsPremium
            }).OrderBy(p => p.CreatedOn).ToListAsync();

            var request = new StorySubCreateReq();
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
