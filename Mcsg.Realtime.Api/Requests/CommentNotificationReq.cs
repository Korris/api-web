using AutoMapper;

namespace Mcsg.Realtime.Api.Requests
{
    using Constants;
    using Lib.Data.Enums;
    using Mappings;

    public class CommentNotificationReq : PostCommentResp, IMapFrom<PostCommentResp>, IMapFrom<ReplyCommentResp>
    {
        public bool IsReply { get; set; }
        public Guid? ReplyToCommentId { get; set; }
        public NotificationEntityType EntityType { get; set; } = NotificationEntityType.PostComment;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PostCommentResp, CommentNotificationReq>()
                .ForMember(d => d.EntityType, opt => opt.MapFrom(s => MapEntityType(s.Type, false)))
                .ForMember(d => d.ReplyToCommentId, opt => opt.Ignore())
            ;
            profile.CreateMap<ReplyCommentResp, CommentNotificationReq>()
                .ForMember(d => d.IsReply, opt => opt.MapFrom(s => s.ReplyToCommentId != Guid.Empty))
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.ReplyToCommentId, opt => opt.MapFrom(s => s.ReplyToCommentId))
                .ForMember(d => d.CommentText, opt => opt.MapFrom(s => s.ReplyText))
                .ForMember(d => d.CommentDate, opt => opt.MapFrom(s => s.ReplyDate))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type))
                .ForMember(d => d.PostId, opt => opt.MapFrom(s => s.PostId))
                .ForMember(d => d.ResourceHashId, opt => opt.MapFrom(s => s.ResourceHashId))
                .ForMember(d => d.ResourceUrl, opt => opt.MapFrom(s => s.ResourceUrl))
                .ForMember(d => d.GifId, opt => opt.MapFrom(s => s.GifId))
                .ForMember(d => d.AuthorId, opt => opt.MapFrom(s => s.AuthorId))
                .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.AuthorName))
                .ForMember(d => d.UserAvatar, opt => opt.MapFrom(s => s.UserAvatar))
                .ForMember(d => d.EntityType, opt => opt.MapFrom(s => MapEntityType(s.Type, true)))
            ;
        }
        private NotificationEntityType MapEntityType(string type, bool isReply)
        {
            if (type == PostTypes.Post)
            {
                return !isReply ? NotificationEntityType.PostComment : NotificationEntityType.PostCommentReply;
            }
            else
            {
                return !isReply ? NotificationEntityType.SubPostComment : NotificationEntityType.SubPostCommentReply;
            }
        }
    }
}
