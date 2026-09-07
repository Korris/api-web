namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Common.Core.Enums;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Responses;

public interface IMentionService
{
    Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, EntityType entityType, int length, int offset, string text);
    Task<bool> AddUserMentionOnComment(Guid commentId, MentionLocationType locationType, AuthorDto author, IEnumerable<MentionDto> mentions, PostDto post);
}
