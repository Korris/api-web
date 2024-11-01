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

namespace Mcsg.Story.Api.Commands;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class PostSyncToAnaH : BaseSettingH, IRequestHandler<PostSyncToAnaR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public PostSyncToAnaH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostSyncToAnaR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new PostSyncToAnaV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            res.SetError(E000, M000, t);
            return res;
        }

        var rsp = await SyncCreateToAna();
        res.SetSuccess(rsp);

        return res;
    }

    private async Task<StoryCreateRsp> SyncCreateToAna()
    {
        var res = new StoryCreateRsp { Success = true };

        var l = _context.StoryPosts.Select(ett => new StoryProtoDto
        {
            PostId = ett.Id.ToString(),
            HashId = ett.HashId,
            UserId = ett.UserId.ToString(),
            Title = ett.Title,
            CreatedOn = ett.CreatedOn.ToString(),
            CreatedBy = ett.CreatedBy.ToString()
        }).OrderBy(p => p.CreatedOn).ToList();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);

            var client = new StoryProto.StoryProtoClient(channel);
            var request = new StoryCreateReq();
            request.Items.AddRange(l);

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
