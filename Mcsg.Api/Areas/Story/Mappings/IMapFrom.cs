using AutoMapper;

namespace Mcsg.Api.Areas.Story.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
