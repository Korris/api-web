namespace Mcsg.Realtime.Api.Interfaces;

using Common.Core.Enums;
using Requests;

public interface IResourceCommentService
{
    Task<ResourceCommentResp?> AddResourceToComment(string userFolder, string hashId, ResourceLocationType locationType, string microService);
}
