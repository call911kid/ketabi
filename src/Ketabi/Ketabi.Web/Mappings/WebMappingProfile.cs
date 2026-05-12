using AutoMapper;
using Ketabi.Application.DTOs.Users;
using Ketabi.Web.ViewModels.Profile;
using Ketabi.Web.ViewModels.Shared;

namespace Ketabi.Web.Mappings;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<UserProfileDto, ProfileViewModel>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.FullName, o => o.MapFrom(s => string.Join(' ', new[] { s.FirstName, s.LastName }.Where(x => !string.IsNullOrWhiteSpace(x)))))
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.AvatarUrl))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.Location))
            .ForMember(d => d.MemberSince, o => o.MapFrom(s => s.MemberSince))
            .ForMember(d => d.ReputationScore, o => o.MapFrom(s => s.Stats.ReputationScore))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Stats.ReviewCount))
            .ForMember(d => d.BooksListed, o => o.MapFrom(s => s.Stats.BooksListed))
            .ForMember(d => d.ActiveListings, o => o.MapFrom(s => s.Stats.BooksListed))
            .ForMember(d => d.CompletedBorrows, o => o.Ignore())
            .ForMember(d => d.CompletedExchanges, o => o.Ignore())
            .ForMember(d => d.IsOwnProfile, o => o.MapFrom(s => s.IsOwnProfile));

        CreateMap<EditProfileViewModel, UpdateUserProfileDto>()
            // Ignore file -> url mapping; controller handles upload and sets the URL explicitly
            .ForMember(d => d.ProfilePictureUrl, o => o.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // UserProfileDto -> EditProfileViewModel (for pre-populating edit form)
        CreateMap<UserProfileDto, EditProfileViewModel>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.Bio))
            .ForMember(d => d.City, o => o.MapFrom(s => s.Location))
            .ForMember(d => d.Governorate, o => o.Ignore())
            .ForMember(d => d.ProfilePicture, o => o.Ignore());

        CreateMap<UserSummaryDto, UserSummaryViewModel>();
    }
}
