using AutoMapper;
using SmartFood.API.Contracts.Auth.Requests;
using SmartFood.Infrastructure.Commands.Auth;

namespace SmartFood.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, RegisterCommand>();
    }
}
