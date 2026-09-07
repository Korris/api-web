namespace Mcsg.Api.Areas.Comic.Interfaces;

using Mcsg.Api.Areas.Comic.Dtos;

public interface IPostLinkService
{
    Task<PostLinkDto> AddLinkAsync(Guid postId, string content);
    Task<bool> RemoveLinkAsync(Guid postId);
}
