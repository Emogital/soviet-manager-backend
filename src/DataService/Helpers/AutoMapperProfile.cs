using AutoMapper;
using DataService.Dtos;
using DataService.Models;

namespace DataService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LobbySettings, LobbySettingsDto>().ReverseMap();
        }
    }
}
