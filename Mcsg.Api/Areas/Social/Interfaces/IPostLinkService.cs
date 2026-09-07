namespace Mcsg.Api.Areas.Social.Interfaces;

using Mcsg.Api.Areas.Social.Dtos;

public interface IPostLinkService
{
    Task<PostLinkDto> AddLinkAsync(Guid postId, string content);
    Task<bool> RemoveLinkAsync(Guid postId);
}
