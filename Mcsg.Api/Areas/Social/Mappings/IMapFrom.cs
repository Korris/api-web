using AutoMapper;

namespace Mcsg.Api.Areas.Social.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
