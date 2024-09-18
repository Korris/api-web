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
                            LEFT JOIN {_notiObjRepository.TableName} obj ON noti.""NotificationObjectId"" = obj.""Id""
                            LEFT JOIN {_userRepository.TableName} us ON obj.""ActorId"" = us.""Id""
                            LEFT JOIN {_subPostReactionRepository.TableName} subpr ON obj.""EntityId"" = subpr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SubPostReaction}
                            LEFT JOIN {_postReactionRepository.TableName} pr ON obj.""EntityId"" = pr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.PostReaction}
                            LEFT JOIN {_comicPostReactionRepository.TableName} cpr ON obj.""EntityId"" = cpr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.ComicPostReaction}
                            LEFT JOIN {_storyPostReactionRepository.TableName} spr ON obj.""EntityId"" = spr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.StoryPostReaction}
                            LEFT JOIN {_postCommentReactionRepository.TableName} pcr ON obj.""EntityId"" = pcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.PostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.PostCommentReplyReaction})
                            LEFT JOIN {_comicPostCommentReactionRepository.TableName} cpcr ON obj.""EntityId"" = cpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.ComicPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.ComicPostCommentReplyReaction})
                            LEFT JOIN {_storyPostCommentReactionRepository.TableName} spcr ON obj.""EntityId"" = spcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.StoryPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.StoryPostCommentReplyReaction})
                            LEFT JOIN {_subPostCommentReactionRepository.TableName} subpcr ON obj.""EntityId"" = subpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.SubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.SubPostCommentReplyReaction})
                            LEFT JOIN {_comicSubPostCommentReactionRepository.TableName} csubpcr ON obj.""EntityId"" = csubpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.ComicSubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.ComicSubPostCommentReplyReaction})
                            LEFT JOIN {_storySubPostCommentReactionRepository.TableName} ssubpcr ON obj.""EntityId"" = ssubpcr.""Id"" AND (obj.""EntityType"" = {(int)NotificationEntityType.StorySubPostCommentReaction} OR obj.""EntityType"" = {(int)NotificationEntityType.StorySubPostCommentReplyReaction})
                            WHERE noti.""ReceiverId"" = @ReceiverId [UnreadCondition]
                            ORDER BY noti.""CreatedOn"" DESC
                            LIMIT @PageSize
                            OFFSET @Offet ;

                            SELECT COUNT(""Id"") FROM {_notiRepository.TableName} 
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
