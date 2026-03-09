using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            //Source => Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishedDto, Platform>().ForMember(dest => dest.ExternalId, opt => opt.MapFrom(sourceMember => sourceMember.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(sourceMember => sourceMember.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(sourceMember => sourceMember.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());

        }
    }
}