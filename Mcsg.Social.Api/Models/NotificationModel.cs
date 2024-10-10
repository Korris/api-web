using AutoMapper;
using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Extensions;
using Mappings;

public class NotificationModel : IMapFrom<NotificationQueryResult>
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public string TargetType { get; set; }
    public Guid? LocationId { get; set; }
    public string LocationHashId { get; set; }
    public Guid? EntityId { get; set; }
    public string EntityHashId { get; set; }
    public string Message { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; }
    public float Order { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public string NotificationType { get; set; }
    public string Avatar { get; set; }
    public int? ReactionType { get; set; }
    public string UserName { get; set; }
    public Guid? CommentId { get; set; }
    public Guid? ReplyCommentId { get; set; }
    public NotificationEntityType NotificationEntityType { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Amount { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<NotificationQueryResult, NotificationModel>()
      .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
      .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToDisplay()))
      .ForMember(d => d.LocationId, opt => opt.MapFrom(s => s.LocationId))
      .ForMember(d => d.LocationHashId, opt => opt.MapFrom(s => s.LocationHashId))
      .ForMember(d => d.EntityId, opt => opt.MapFrom(s => s.EntityId))
      .ForMember(d => d.EntityHashId, opt => opt.MapFrom(s => s.EntityHashId))
      .ForMember(d => d.ActorId, opt => opt.MapFrom(s => s.ActorId))
      .ForMember(d => d.ActorName, opt => opt.MapFrom(s => s.ActorName))
      .ForMember(d => d.CreatedOn, opt => opt.MapFrom(s => s.CreatedOn))
      .ForMember(d => d.CommentId, opt => opt.MapFrom(s => s.EntityType.ToString().Contains("Comment", StringComparison.OrdinalIgnoreCase) ? s.EntityId : null))
      .ForMember(d => d.Message, opt => opt.MapFrom(s => s.ToMessage()))
      .ForMember(d => d.TargetType, opt => opt.MapFrom(s => s.ToTargetType()))
      .ForMember(d => d.NotificationType, opt => opt.MapFrom(s => s.ToNotiType()))
      .ForMember(d => d.NotificationEntityType, opt => opt.MapFrom(s => s.EntityType))
      .ForMember(d => d.Avatar, opt => opt.MapFrom(s => s.Avatar))
      .ForMember(d => d.ReactionType, opt => opt.MapFrom(s => s.ReactionType))
      .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
  ;
    }
}
