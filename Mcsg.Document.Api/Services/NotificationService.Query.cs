namespace Mcsg.Document.Api.Services
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
                            , (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName"" ELSE us.""ProfileName"" END) AS ActorName
                            , us.""Avatar""
                            , COALESCE(pr.""Type"", COALESCE(spr.""Type"", COALESCE(pcr.""Type"", COALESCE(pr.""Type"", spcr.""Type"")))) AS ""ReactionType""
                            FROM {_notiRepository.TableName} noti
                            LEFT JOIN {_notiObjRepository.TableName} obj ON noti.""NotificationObjectId"" = obj.""Id""
                            LEFT JOIN {_userRepository.TableName} us ON obj.""ActorId"" = us.""Id""
                            LEFT JOIN {_postReacRepository.TableName} pr ON obj.""EntityId"" = pr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.PostReaction}
                            LEFT JOIN {_subPostReacRepository.TableName} spr ON obj.""EntityId"" = spr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SubPostReaction}
                            LEFT JOIN {_postCommentRepository.TableName} pcr ON obj.""EntityId"" = pcr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.PostCommentReaction}
                            LEFT JOIN {_subPostCommentRepository.TableName} spcr ON obj.""EntityId"" = spcr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SubPostCommentReaction}
                            WHERE noti.""ReceiverId"" = @ReceiverId 
                            ORDER BY noti.""CreatedOn"" DESC
                            LIMIT @PageSize
                            OFFSET @Offet ;

                            SELECT COUNT(""Id"") FROM {_notiRepository.TableName} 
                            WHERE ""ReceiverId"" = @ReceiverId ;";
            }
        }
        private string GetNotificationUnReadByUserQuery
        {
            get
            {
                return @$"SELECT noti.""Id"", noti.""ReceiverId""
                            , noti.""Status"", obj.""LocationId"", obj.""LocationHashId""
                            , obj.""EntityType"", obj.""EntityId"", obj.""EntityHashId"", obj.""Action"", obj.""CreatedOn""
                            , obj.""ActorId""
                            , (CASE WHEN us.""ProfileName"" IS NULL THEN us.""UserName"" ELSE us.""ProfileName"" END) AS ActorName
                            , us.""Avatar""
                            , COALESCE(pr.""Type"", COALESCE(spr.""Type"", COALESCE(pcr.""Type"", COALESCE(pr.""Type"", spcr.""Type"")))) AS ""ReactionType""
                            FROM {_notiRepository.TableName} noti
                            LEFT JOIN {_notiObjRepository.TableName} obj ON noti.""NotificationObjectId"" = obj.""Id""
                            LEFT JOIN {_userRepository.TableName} us ON obj.""ActorId"" = us.""Id""
                            LEFT JOIN {_postReacRepository.TableName} pr ON obj.""EntityId"" = pr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.PostReaction}
                            LEFT JOIN {_subPostReacRepository.TableName} spr ON obj.""EntityId"" = spr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SubPostReaction}
                            LEFT JOIN {_postCommentRepository.TableName} pcr ON obj.""EntityId"" = pcr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.PostCommentReaction}
                            LEFT JOIN {_subPostCommentRepository.TableName} spcr ON obj.""EntityId"" = spcr.""Id"" AND obj.""EntityType"" = {(int)NotificationEntityType.SubPostCommentReaction}
                            WHERE noti.""ReceiverId"" = @ReceiverId AND noti.""Status"" = 0 
                            ORDER BY noti.""CreatedOn"" DESC
                            LIMIT @PageSize
                            OFFSET @Offet ;

                            SELECT COUNT(""Id"") FROM {_notiRepository.TableName} 
                            WHERE ""ReceiverId"" = @ReceiverId  AND ""Status"" = 0 ;";
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
