using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class ReviewProfiles : Profile
{
    public ReviewProfiles()
    {
        CreateMap<Review, ReviewCreateDTO>();
        CreateMap<ReviewCreateDTO, Review>();
        CreateMap<ReviewDTO, Review>();
        CreateMap<Review, ReviewDTO>();
        CreateMap<ReviewUpdateDTO, Review>();
        CreateMap<Review, ReviewUpdateDTO>();
    }
}
