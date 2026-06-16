using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class ActorProfiles : Profile
{
    public ActorProfiles()
    {
        CreateMap<Actor, ActorCreateDTO>();
        CreateMap<ActorCreateDTO, Actor>();
        CreateMap<ActorDTO, Actor>();
        CreateMap<Actor, ActorDTO>();
        CreateMap<ActorUpdateDTO, Actor>();
        CreateMap<Actor, ActorUpdateDTO>();
    }
}
