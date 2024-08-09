namespace Mcsg.Realtime.Api.Interfaces;

using Common.Core.Enums;
using Dtos;

public interface IMentionService
{
    Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, EntityType entityType, int length, int offset, string text);
    Task<bool> AddUserMentionOnComment(Guid commentId, MentionLocationType locationType, AuthorModel author, IEnumerable<MentionDto> mentions, PostDto post);
}
