using Grpc.Net.Client;

namespace Mcsg.Api.Areas.TapShow.Services;

using Analytic.Application.Protos;
using Common.Domain.Entities;
using Common.Core.Extensions;

/// <summary>
/// Mirrors TapShow posts into the Analytic service (sync.tap_show_posts) so the home feed ranking can return them.
/// Fire-and-forget from the callers, same as Comic / Story / Document: a failure is logged and never fails the request.
/// </summary>
public partial class TapShowPostService
{
    #region -- Helpers --

    private async Task SyncCreateToAna(TapShowPost post)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new TapShowProto.TapShowProtoClient(channel);
            var request = new TapShowCreateReq
            {
                Items =
                {
                    new TapShowProtoDto
                    {
                        PostId = post.Id.ToString(),
                        HashId = post.HashId,
                        UserId = post.UserId.ToString(),
                        Title = post.Title ?? string.Empty,
                        Permission = (int)post.Permission,
                        CreatedOn = post.CreatedOn.ToString(),
                        CreatedBy = post.CreatedBy?.ToString(),
                        ModifiedOn = post.ModifiedOn?.ToString(),
                        ModifiedBy = post.ModifiedBy?.ToString()
                    }
                }
            };
            var rsp = await client.CreateAsync(request);
            LogSync("create", post.Id, rsp.Success, rsp.Message);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    private async Task SyncUpdateToAna(TapShowPost post)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new TapShowProto.TapShowProtoClient(channel);
            var request = new TapShowUpdateReq
            {
                PostId = post.Id.ToString(),
                Title = post.Title ?? string.Empty,
                Permission = (int)post.Permission,
                ModifiedOn = post.ModifiedOn?.ToString(),
                ModifiedBy = post.ModifiedBy?.ToString()
            };
            var rsp = await client.UpdateAsync(request);
            LogSync("update", post.Id, rsp.Success, rsp.Message);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    private async Task SyncDeleteToAna(Guid postId)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new TapShowProto.TapShowProtoClient(channel);
            var rsp = await client.DeleteAsync(new TapShowDeleteReq { PostId = postId.ToString() });
            LogSync("delete", postId, rsp.Success, rsp.Message);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    private static void LogSync(string action, Guid postId, bool success, string? message)
    {
        if (!success)
        {
            $"[TAPSHOW-SYNC] {action} {postId} not applied by Analytic: {message}".LogError();
        }
    }

    #endregion
}
