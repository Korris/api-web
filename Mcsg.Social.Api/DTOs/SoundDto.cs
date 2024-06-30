using AutoMapper;

namespace Mcsg.Social.Api.DTOs
{
    using Extensions;
    using Lib.Data.Domain.Entities;
    using Mappings;

    public class SoundDto : IMapFrom<BackgroundMedia>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public string ArtistName { get; set; }
        public string Duration { get; set; }
        public int Order { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<BackgroundMedia, SoundDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.Url, opt => opt.MapFrom(s => s.Url.ToAudioPath()))
                .ForMember(d => d.Thumbnail, opt => opt.MapFrom(s => s.Thumbnail.ToImagePath()))
                .ForMember(d => d.ArtistName, opt => opt.MapFrom(s => s.ArtistName))
                .ForMember(d => d.Duration, opt => opt.MapFrom(s => s.DurationSeconds.ToDuration()))
                .ForMember(d => d.Order, opt => opt.MapFrom(s => s.Order))
            ;
        }
    }
}
