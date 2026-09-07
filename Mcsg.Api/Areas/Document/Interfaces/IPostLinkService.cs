namespace Mcsg.Api.Areas.Document.Interfaces;

using Mcsg.Api.Areas.Document.Dtos;

public interface IPostLinkService
{
    Task<PostLinkDto> AddLinkAsync(Guid postId, string content);
    Task<bool> RemoveLinkAsync(Guid postId);
}
