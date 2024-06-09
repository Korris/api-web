using AutoMapper;
using Mcsg.Social.Api.Mappings;

namespace Mcsg.Social.Api.Models.Earning
{
    public class ReportSeriesData : IMapFrom<ChapterTOCExtendResponse>
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public string Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public int Views { get; set; }
        public int Purchases { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChapterTOCExtendResponse, ReportSeriesData>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.No, opt => opt.MapFrom(s => s.Order))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.PublishDate, opt => opt.MapFrom(s => s.PublishDate))
            ;
        }
    }
}
