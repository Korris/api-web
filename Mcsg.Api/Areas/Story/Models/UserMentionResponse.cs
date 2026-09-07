using AutoMapper;

namespace Mcsg.Api.Areas.Story.Models;

using Mcsg.Api.Areas.Story.Mappings;

public class UserMentionResponse : IMapFrom<UserMentionModel>
{
    public Guid UserId { get; set; }
    public string ProfileName { get; set; }
    public string? UserName { get; set; }
    public string ProfileId { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserMentionModel, UserMentionResponse>()
            .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.EntityId))
            .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.ProfileName))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName))
            .ForMember(d => d.Length, opt => opt.MapFrom(s => s.Length))
            .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
        ;
    }
}
