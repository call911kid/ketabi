using AutoMapper;
using Ketabi.Application.DTOs.Notifications;
using Ketabi.Core.Domain.Entities;

namespace Ketabi.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.NotificationId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.TimeAgo,        o => o.MapFrom(s => GetTimeAgo(s.CreatedAt)));

        CreateMap<CreateNotificationDto, Notification>()
            .ForMember(d => d.IsRead, o => o.MapFrom(_ => false));
    }

    private static string GetTimeAgo(DateTime createdAt)
    {
        var diff = DateTime.UtcNow - createdAt;
        if (diff.TotalMinutes < 1)  return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours   < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays    < 7)  return $"{(int)diff.TotalDays}d ago";
        return createdAt.ToString("MMM d");
    }
}
