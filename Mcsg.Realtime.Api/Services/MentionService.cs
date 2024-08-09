using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Enums;
    using Common.Domain.Entities;
    using Dtos;
    using Interfaces;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Requests;

    public partial class MentionService : IMentionService
    {
        private readonly IRepository<Mention> _mentionRepository;
        private readonly INotificationService _notificationService;
        public MentionService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _mentionRepository = unitOfWork.GetRepository<Mention>();
            _notificationService = notificationService;
        }

        public async Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, EntityType entityType, int length, int offset, string text)
        {
            var mention = new Mention()
            {
                LocationId = locationId,
                LocationType = locationType,
                EntityId = entityId,
                EntityType = entityType,
                Length = length,
                Offset = offset,
                Text = text
            };
            await _mentionRepository.InsertAsync(mention);

            return new MentionResp();
        }

        public async Task<bool> AddUserMentionOnComment(Guid commentId, MentionLocationType locationType, AuthorModel author, IEnumerable<MentionDto> mentions, PostDto post)
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
                var mention = new Mention()
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

            // Insert List
            if (mentionInsert.Count > 0)
            {
                var result = await _mentionRepository.InsertAsync(mentionInsert);
                if (result > 0)
                {
                    foreach (var mention in mentionInsert)
                    {
                        var mentionNoti = new MentionNotificationReq()
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
    }
}
