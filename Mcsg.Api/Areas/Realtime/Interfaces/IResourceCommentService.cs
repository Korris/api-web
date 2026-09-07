namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Requests;

public interface IResourceCommentService
{
    Task<ResourceCommentResp?> AddResourceToComment(ResourceCommentDto dto);
}
