using AutoMapper;
using Mcsg.Social.Api.Mappings;

namespace Mcsg.Social.Api.Models
{
    public class UserMentionResponse : IMapFrom<UserMentionModel>
    {
        public Guid UserId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileId { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserMentionModel, UserMentionResponse>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.EntityId))
                .ForMember(d => d.ProfileName, opt => opt.MapFrom(s => s.ProfileName))
                .ForMember(d => d.Length, opt => opt.MapFrom(s => s.Length))
                .ForMember(d => d.Offset, opt => opt.MapFrom(s => s.Offset))
            ;
        }
    }
}
