using AutoMapper;

namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Enums;
using Constants;
using Mappings;

public class CommentNotificationReq : PostCommentResp, IMapFrom<PostCommentResp>, IMapFrom<ReplyCommentResp>
{
    public Guid CommentId { get; set; }
    public bool IsReply { get; set; }
    public Guid? ReplyToCommentId { get; set; }
    public Guid? QuoteId { get; set; }
    public NotificationEntityType EntityType { get; set; } = NotificationEntityType.PostComment;
    public string? PostHashId { get; set; }
    public string LocationHashId { get; set; }
    public string MicroServiceType { get; set; } = MicroService.Social.ToString();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PostCommentResp, CommentNotificationReq>()
            .ForMember(d => d.EntityType, opt => opt.MapFrom(s => MapEntityType(s.Type, false, s.PostType)))
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
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.EntityType, opt => opt.MapFrom(s => MapEntityType(s.Type, true, s.PostType)))
        ;
    }

    private NotificationEntityType MapEntityType(string type, bool isReply, PostType postType)
    {
        switch (postType)
        {
            case PostType.Comic:
                if (type == PostTypes.Post)
                {
                    return !isReply ? NotificationEntityType.ComicPostComment : NotificationEntityType.ComicPostCommentReply;
                }
                else
                {
                    return !isReply ? NotificationEntityType.ComicSubPostComment : NotificationEntityType.ComicSubPostCommentReply;
                }

            case PostType.Story:
                if (type == PostTypes.Post)
                {
                    return !isReply ? NotificationEntityType.StoryPostComment : NotificationEntityType.StoryPostCommentReply;
                }
                else
                {
                    return !isReply ? NotificationEntityType.StorySubPostComment : NotificationEntityType.StorySubPostCommentReply;
                }

            default:
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
