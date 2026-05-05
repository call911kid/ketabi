using AutoMapper;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Web.ViewModels.Account;

namespace Ketabi.Web.Mappings;

public class AccountWebProfile : Profile
{
    public AccountWebProfile()
    {
        CreateMap<LoginViewModel, LoginRequest>();

        CreateMap<RegisterViewModel, RegisterRequest>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());
    }
}

