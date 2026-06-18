using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class ActorProfiles : Profile
{
    public ActorProfiles()
    {
        CreateMap<Movie, ActorCreateDTO>();
        CreateMap<ActorCreateDTO, Movie>();
        CreateMap<ActorDTO, Movie>();
        CreateMap<Movie, ActorDTO>();
        CreateMap<ActorUpdateDTO, Movie>();
        CreateMap<Movie, ActorUpdateDTO>();
    }
}
