using AutoMapper;

namespace Mcsg.Story.Api.Models;

using Extensions;
using Mappings;

public class CommentResponse : IMapFrom<CommentQueryModel>
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime? ModifiedOn { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public ReplyResponse Replies { get; set; } = new ReplyResponse();
    public List<UserMentionResponse> Mentions { get; set; } = new List<UserMentionResponse>();
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CommentQueryModel, CommentResponse>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.PostId, opt => opt.MapFrom(s => s.PostId))
            .ForMember(d => d.AuthorId, opt => opt.MapFrom(s => s.AuthorId))
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.AuthorName))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.UserAvatar, opt => opt.MapFrom(s => s.UserAvatar.ToPublicImageUrl()))
            .ForMember(d => d.Body, opt => opt.MapFrom(s => s.Body))
            .ForMember(d => d.ModifiedOn, opt => opt.MapFrom(s => s.ModifiedOn))
            .ForMember(d => d.ResourceUrl, opt => opt.Ignore())
            .ForMember(d => d.Replies, opt => opt.Ignore())
            .ForMember(d => d.Mentions, opt => opt.Ignore())
            .ReverseMap();
    }
}
