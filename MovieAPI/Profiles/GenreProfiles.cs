using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class GenreProfiles : Profile
{
    public GenreProfiles()
    {
        CreateMap<Genre, GenreCreateDTO>();
        CreateMap<GenreCreateDTO, Genre>();
        CreateMap<GenreDTO, Genre>();
        CreateMap<Genre, GenreDTO>();
        CreateMap<GenreUpdateDTO, Genre>();
        CreateMap<Genre, GenreUpdateDTO>();
    }
}
