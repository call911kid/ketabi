using AutoMapper;
using Ketabi.Application.DTOs.Auth;
using Ketabi.Application.DTOs.Users;
using Ketabi.Web.ViewModels.Account;
using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.Mappings;

public class AccountWebProfile : Profile
{
    public AccountWebProfile()
    {
        CreateMap<LoginViewModel, LoginRequest>();

        CreateMap<RegisterViewModel, RegisterRequest>()
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());

        // Map UserSummaryDto to UserSummaryViewModel (used in book details page)
        CreateMap<UserSummaryDto, UserSummaryViewModel>();
    }
}

