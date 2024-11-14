namespace Mcsg.Social.Api.Services
{
    using Common.Core.Enums;

    public partial class NotificationService
    {
        private string GetNotificationByUserQuery
        {
            get
            {
                return @$"SELECT noti.""Id"", noti.""ReceiverId""
                            , noti.""Status"", obj.""LocationId"", obj.""LocationHashId""
                            , obj.""EntityType"", obj.""EntityId"", obj.""EntityHashId"", obj.""Action"", obj.""CreatedOn""
                            , obj.""ActorId""
                            , us.""UserName""
                            , (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName"" ELSE us.""ProfileName"" END) AS ActorName
                            , us.""Avatar""
                            , COALESCE(subpr.""Type""
                            , COALESCE(pr.""Type""
                            , COALESCE(cpr.""Type""
                            , COALESCE(spr.""Type""
                            , COALESCE(pcr.""Type""
                            , COALESCE(cpcr.""Type""
                            , COALESCE(spcr.""Type""
                            , COALESCE(subpcr.""Type""
                            , COALESCE(csubpcr.""Type""
                            , COALESCE(ssubpcr.""Type""
                            , COALESCE(pr.""Type"", spr.""Type""))))))))))) AS ""ReactionType""
                            FROM {_notiRepository.TableName} noti
                            LEFT JOIN system.""NotificationObjects"" obj ON noti.""NotificationObjectId"" = obj.""Id""
                            RIGHT JOIN identity.""Users"" us ON obj.""ActorId"" = us.""Id"" AND us.""IsDelete"" = false
                            LEFT JOIN social.""SocialSubPostReactions"" subpr ON obj.""EntityId"" = subpr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SocialSubPostReaction}
                            LEFT JOIN social.""SocialPostReactions"" pr ON obj.""EntityId"" = pr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SocialPostReaction}
                            LEFT JOIN comic.""ComicPostReactions"" cpr ON obj.""EntityId"" = cpr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.ComicPostReaction}
                            LEFT JOIN story.""StoryPostReactions"" spr ON obj.""EntityId"" = spr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.StoryPostReaction}
                            LEFT JOIN social.""SocialPostCommentReactions"" pcr ON obj.""EntityId"" = pcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.SocialPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.SocialPostCommentReplyReaction})
                            LEFT JOIN comic.""ComicPostCommentReactions"" cpcr ON obj.""EntityId"" = cpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.ComicPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.ComicPostCommentReplyReaction})
                            LEFT JOIN story.""StoryPostCommentReactions"" spcr ON obj.""EntityId"" = spcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.StoryPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.StoryPostCommentReplyReaction})
                            LEFT JOIN social.""SocialSubPostCommentReactions"" subpcr ON obj.""EntityId"" = subpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.SocialSubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.SocialSubPostCommentReplyReaction})
                            LEFT JOIN comic.""ComicSubPostCommentReactions"" csubpcr ON obj.""EntityId"" = csubpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.ComicSubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.ComicSubPostCommentReplyReaction})
                            LEFT JOIN story.""StorySubPostCommentReactions"" ssubpcr ON obj.""EntityId"" = ssubpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.StorySubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.StorySubPostCommentReplyReaction})
                            WHERE noti.""ReceiverId"" = @ReceiverId [UnreadCondition]
                            ORDER BY noti.""CreatedOn"" DESC
                            LIMIT @PageSize
                            OFFSET @Offet ;

                            SELECT COUNT(noti.""Id"") FROM {_notiRepository.TableName} noti
                            LEFT JOIN system.""NotificationObjects"" obj ON noti.""NotificationObjectId"" = obj.""Id""
                            RIGHT JOIN identity.""Users"" us ON obj.""ActorId"" = us.""Id"" AND us.""IsDelete"" = false
                            WHERE ""ReceiverId"" = @ReceiverId [UnreadCountCondition] ;";
            }
        }

        private string UpdateNotificationStatusQuery
        {
            get
            {
                return @$"UPDATE {_notiRepository.TableName}
                                SET ""Status"" = @Status
                                WHERE ""ReceiverId"" = @ReceiverId ";
            }
        }
    }
}
