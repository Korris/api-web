using Dapper;

namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;
using Mcsg.Api.Areas.Realtime.Responses;

public partial class MentionService : BaseS, IMentionService
{
    /// <summary>
    /// Initialize
    /// </summary>
    public MentionService(IMcsgContext context, IUnitOfWork unitOfWork, INotificationService notificationService) : base(context)
    {
        _mentionRepository = unitOfWork.GetRepository<Mention>();
        _notificationService = notificationService;
    }

    public async Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, EntityType entityType, int length, int offset, string text)
    {
        var mention = new Mention
        {
            LocationId = locationId,
            LocationType = locationType,
            EntityId = entityId,
            EntityType = entityType,
            Length = length,
            Offset = offset,
            Text = text
        };
        await _context.Mentions.AddAsync(mention);
        await _context.SaveChangesAsync(default);

        return new MentionResp();
    }

    public async Task<bool> AddUserMentionOnComment(Guid commentId, MentionLocationType locationType, AuthorDto author, IEnumerable<MentionDto> mentions, PostDto post)
    {
        // Remove all mention of comment first
        await _mentionRepository.Connection.ExecuteAsync(DeleteMentionCommentCommand,
                                        new
                                        {
                                            LocationId = commentId,
                                            ModifiedBy = author.Id,
                                            ModifiedOn = DateTime.UtcNow
                                        });

        var mentionInsert = new List<Mention>();
        foreach (var item in mentions)
        {
            var mention = new Mention
            {
                LocationId = commentId,
                LocationType = locationType,
                EntityId = item.EntityId,
                EntityType = EntityType.User,
                Length = item.Length,
                Offset = item.Offset,
                Text = item.Text
            };

            mentionInsert.Add(mention);
        }

        if (mentionInsert.Count > 0)
        {
            await _context.Mentions.AddRangeAsync(mentionInsert);
            var result = await _context.SaveChangesAsync(default);
            if (result > 0)
            {
                foreach (var mention in mentionInsert)
                {
                    var mentionNoti = new MentionNotificationReq
                    {
                        LocationId = commentId,
                        LocationType = locationType,
                        EntityId = mention.EntityId,
                        EntityType = mention.EntityType,
                        AuthorId = author.Id,
                        AuthorName = author.Name,
                        UserAvatar = author.Avatar,
                        PostHashId = post.HashId
                    };
                    await _notificationService.AddMentionNotification(mentionNoti);
                }
            }
            return result > 0;
        }

        return false;
    }

    #region -- Fields --

    private readonly IRepository<Mention> _mentionRepository;
    private readonly INotificationService _notificationService;

    #endregion
}
