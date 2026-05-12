using AutoMapper;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Users;
using Ketabi.Core.Domain.Entities;

namespace Ketabi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User -> UserProfileDto (basic fields). Stats are populated in service.
        CreateMap<User, UserProfileDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ProfilePictureUrl) ? AppConstants.DefaultProfilePic : s.ProfilePictureUrl))
            .ForMember(d => d.Location, o => o.MapFrom(s => BuildLocation(s.City, s.Governorate)))
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName));

        // User -> UserSummaryDto
        CreateMap<User, UserSummaryDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.FullName, o => o.MapFrom(s => (s.FirstName + " " + s.LastName).Trim()))
            .ForMember(d => d.UserName, o => o.MapFrom(s => EmailToUserName(s.Email)))
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.ProfilePictureUrl) ? AppConstants.DefaultProfilePic : s.ProfilePictureUrl))
            .ForMember(d => d.Location, o => o.MapFrom(s => BuildLocation(s.City, s.Governorate)))
            .ForMember(d => d.ReputationScore, o => o.MapFrom(s => s.ReputationScore));

        // User -> UserReputationDto
        CreateMap<User, UserReputationDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ReputationScore, o => o.MapFrom(s => s.ReputationScore));

        // CreateUserDto -> User
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id == Guid.Empty ? Guid.NewGuid() : s.Id))
            .ForMember(d => d.ProfilePictureUrl, o => o.MapFrom(s => s.ProfilePictureUrl));

        // UpdateUserProfileDto -> User (partial update)
        CreateMap<UpdateUserProfileDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

    private static string BuildLocation(string? city, string? governorate)
    {
        if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(governorate))
            return string.Empty;

        if (string.IsNullOrWhiteSpace(city)) return governorate ?? string.Empty;
        if (string.IsNullOrWhiteSpace(governorate)) return city;
        return $"{city}, {governorate}";
    }

    private static string EmailToUserName(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;
        var parts = email.Split('@');
        return parts.Length > 0 ? parts[0] : email;
    }
}
