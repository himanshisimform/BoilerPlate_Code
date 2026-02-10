using AutoMapper;
using SimformMicroservice_Project1.Database.Tables;
using SimformMicroservice_Project1.DTO;

namespace SimformMicroservice_Project1.Service.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        CreateMap<ApplicationRole, RoleDto>();

        CreateMap<RegisterRequest, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}