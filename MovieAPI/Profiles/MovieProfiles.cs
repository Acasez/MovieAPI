using AutoMapper;

namespace MovieAPI.Profiles;

public class MovieProfiles : Profile
{
    public MovieProfiles()
    {
        CreateMap<DataTrransferObjects.MovieCreateDTO, Models.Movie>();
        CreateMap<Models.Movie, DataTrransferObjects.MovieCreateDTO>();
    }
}
