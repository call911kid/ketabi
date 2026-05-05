using AutoMapper;
using Ketabi.Application.DTOs.Reviews;
using Ketabi.Application.DTOs.Users;
using Ketabi.Core.Domain.Entities;

namespace Ketabi.Application.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        // ── Review entity → ReviewDto ────────────────────────────────────────
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.ReviewId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.Reviewer,
                opt => opt.MapFrom(src => src.Reviewer))

            .ForMember(dest => dest.Reviewee,
                opt => opt.MapFrom(src => src.TargetUser))

            .ForMember(dest => dest.RelatedBookTitle,
                opt => opt.MapFrom(src =>
                    src.RelatedRequest != null && src.RelatedRequest.Listing != null
                        ? src.RelatedRequest.Listing.Title
                        : string.Empty))

            .ForMember(dest => dest.RelatedRequestId,
                opt => opt.MapFrom(src => src.RelatedRequestId))

            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))

            .ForMember(dest => dest.TimeAgo,
                opt => opt.MapFrom(src => GetTimeAgo(src.CreatedAt)));

        // ── CreateReviewDto → Review entity ─────────────────────────────────
        CreateMap<CreateReviewDto, Review>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(_ => Guid.NewGuid()))

            .ForMember(dest => dest.TargetUserId,
                opt => opt.MapFrom(src => src.RevieweeId))

            // ReviewerId must be set manually from the authenticated identity
            .ForMember(dest => dest.ReviewerId,
                opt => opt.Ignore())

            // Navigation properties are never mapped from a DTO
            .ForMember(dest => dest.Reviewer,      opt => opt.Ignore())
            .ForMember(dest => dest.TargetUser,    opt => opt.Ignore())
            .ForMember(dest => dest.RelatedRequest, opt => opt.Ignore())

            // BaseEntity fields
            .ForMember(dest => dest.CreatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt,  opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted,  opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt,  opt => opt.Ignore());

        // ── User entity → UserSummaryDto ─────────────────────────────────────
        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.Id))

            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))

            // UserName lives in ASP.NET Identity — not on the domain User entity
            .ForMember(dest => dest.UserName,
                opt => opt.Ignore())

            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.ProfilePictureUrl ?? string.Empty))

            .ForMember(dest => dest.Location,
                opt => opt.MapFrom(src => FormatLocation(src.City, src.Governorate)))

            .ForMember(dest => dest.ReputationScore,
                opt => opt.MapFrom(src => src.ReputationScore))

            // ReviewCount requires a DB query — populate after mapping if needed
            .ForMember(dest => dest.ReviewCount,
                opt => opt.Ignore());
    }

    // ── Static helpers ───────────────────────────────────────────────────────

    private static string FormatLocation(string? city, string? governorate) =>
        (city, governorate) switch
        {
            ({ } c, { } g) => $"{c}, {g}",
            ({ } c, null) => c,
            (null, { } g) => g,
            _ => string.Empty,
        };

    private static string GetTimeAgo(DateTime createdAt)
    {
        var elapsed = DateTime.UtcNow - createdAt;

        return elapsed.TotalSeconds switch
        {
            < 60       => "just now",
            < 3600     => $"{(int)elapsed.TotalMinutes} minute{S((int)elapsed.TotalMinutes)} ago",
            < 86400    => $"{(int)elapsed.TotalHours} hour{S((int)elapsed.TotalHours)} ago",
            < 2592000  => $"{(int)elapsed.TotalDays} day{S((int)elapsed.TotalDays)} ago",
            < 31536000 => $"{(int)(elapsed.TotalDays / 30)} month{S((int)(elapsed.TotalDays / 30))} ago",
            _          => $"{(int)(elapsed.TotalDays / 365)} year{S((int)(elapsed.TotalDays / 365))} ago",
        };
    }

    private static string S(int value) => value == 1 ? string.Empty : "s";
}
