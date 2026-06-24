using AutoMapper;
using MovieAPI.DataTransferObjects;
using MovieAPI.Models;

namespace MovieAPI.Profiles;

public class SettingProfiles : Profile
{
    public SettingProfiles()
    {
        CreateMap<Setting, SettingCreateDTO>();
        CreateMap<SettingCreateDTO, Setting>();
        CreateMap<SettingDTO, Setting>();
        CreateMap<Setting, SettingDTO>();
        CreateMap<SettingUpdateDTO, Setting>();
        CreateMap<Setting, SettingUpdateDTO>();
    }
}
