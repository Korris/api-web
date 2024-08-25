namespace Mcsg.Realtime.Api.Interfaces;

using Dtos;
using Requests;

public interface IResourceCommentService
{
    Task<ResourceCommentResp?> AddResourceToComment(ResourceCommentDto dto);
}
