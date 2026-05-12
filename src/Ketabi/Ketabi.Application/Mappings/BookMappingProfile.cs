using AutoMapper;
using Ketabi.Application.DTOs.Books;
using Ketabi.Application.DTOs.Users;
using Ketabi.Core.Domain.Entities;
using System.Linq;

namespace Ketabi.Application.Mappings;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        CreateMap<BookListing, BookSummaryDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.SharingMode, o => o.MapFrom(s => s.SharingMode.ToString()))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.OwnerId, o => o.MapFrom(s => s.User != null ? s.User.Id : Guid.Empty))
            .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
            .ForMember(d => d.OwnerImageUrl, o => o.MapFrom(s => s.User != null ? s.User.ProfilePictureUrl : null))
            .ForMember(d => d.OwnerAvatarUrl, o => o.MapFrom(s => s.User != null ? s.User.ProfilePictureUrl : null))
            .ForMember(d => d.OwnerReputation, o => o.MapFrom(s => s.User != null ? s.User.ReputationScore : 0.0))
            .ForMember(d => d.OwnerRating, o => o.MapFrom(s => s.User != null ? s.User.ReputationScore : 0.0));

        CreateMap<BookListing, BookDetailDto>()
            .ForMember(d => d.BookId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ImageUrls, o => o.MapFrom(s => s.Tags != null ? s.Tags.Select(t => t.Tag).ToList() : new List<string>()))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags != null ? s.Tags.Select(t => t.Tag).ToList() : new List<string>()))
            .ForMember(d => d.Owner, o => o.MapFrom(s => s.User))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<User, UserSummaryDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FirstName + " " + s.LastName))
            .ForMember(d => d.UserName, o => o.MapFrom<EmailToUserNameResolver>())
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.ProfilePictureUrl ?? string.Empty))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.City != null && s.Governorate != null ? s.City + ", " + s.Governorate : s.City ?? s.Governorate ?? string.Empty))
            .ForMember(d => d.ReputationScore, o => o.MapFrom(s => s.ReputationScore))
            .ForMember(d => d.ReviewCount, o => o.Ignore())
            .ForMember(d => d.TradesCount, o => o.Ignore());

        CreateMap<CreateBookDto, BookListing>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags != null ? s.Tags.Select(t => new BookListingTag { Id = Guid.NewGuid(), Tag = t }).ToList() : new List<BookListingTag>()));
    }
}
