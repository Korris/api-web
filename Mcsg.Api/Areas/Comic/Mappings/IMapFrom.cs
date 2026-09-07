using AutoMapper;

namespace Mcsg.Api.Areas.Comic.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
