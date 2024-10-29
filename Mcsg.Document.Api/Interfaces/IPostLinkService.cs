namespace Mcsg.Document.Api.Interfaces;

using Dtos;

public interface IPostLinkService
{
    Task<PostLinkDto> AddLinkAsync(Guid postId, string content);
    Task<bool> RemoveLinkAsync(Guid postId);
}
