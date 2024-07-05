using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Dtos;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Requests;

    public interface IMentionService
    {
        Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, MentionEntityType entityType, int length, int offset, string text);
        Task<bool> AddUserMentionOnComment(Guid commentId, MentionLocationType locationType, AuthorModel author, IEnumerable<MentionDto> mentions, PostDto post);
    }
    public partial class MentionService : IMentionService
    {
        private readonly IRepository<Mention> _mentionRepository;
        private readonly INotificationService _notificationService;
        public MentionService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _mentionRepository = unitOfWork.GetRepository<Mention>();
            _notificationService = notificationService;
        }

        public async Task<MentionResp> AddMention(Guid locationId, MentionLocationType locationType, Guid entityId, MentionEntityType entityType, int length, int offset, string text)
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
                                                LastModifiedBy = author.Id,
                                                LastModifiedDate = DateTime.UtcNow
                                            });

            var mentionInsert = new List<Mention>();
            foreach (var item in mentions)
            {
                var mention = new Mention()
                {
                    LocationId = commentId,
                    LocationType = locationType,
                    EntityId = item.EntityId,
                    EntityType = MentionEntityType.User,
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
