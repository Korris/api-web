using AutoMapper;

namespace Mcsg.Document.Api.Models.Earning;

using Common.Core.Enums;
using Mappings;

public class MyPostSeriesQueryResult
{
    public string HashId { get; set; }
    public string Title { get; set; }
    public PostType Type { get; set; }
    public PostStatus Status { get; set; }
}
public class MyPostSeriesResponse : IMapFrom<MyPostSeriesQueryResult>
{
    public string HashId { get; set; }
    public string Title { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<MyPostSeriesQueryResult, MyPostSeriesResponse>()
            .ForMember(d => d.HashId, opt => opt.MapFrom(s => s.HashId))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
        //.ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToDisplay()))
        //.ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToDisplay()))
        ;
    }
}
