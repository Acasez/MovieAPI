using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class MovieProfiles : Profile
{
    public MovieProfiles()
    {
        CreateMap<Movie, MovieCreateDTO>();
        CreateMap<MovieCreateDTO, Movie>();
    }
}
