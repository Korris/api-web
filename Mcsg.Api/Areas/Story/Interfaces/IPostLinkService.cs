namespace Mcsg.Api.Areas.Story.Interfaces;

using Mcsg.Api.Areas.Story.Dtos;

public interface IPostLinkService
{
    Task<PostLinkDto> AddLinkAsync(Guid postId, string content);
    Task<bool> RemoveLinkAsync(Guid postId);
}
