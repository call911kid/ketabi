using AutoMapper;
using Ketabi.Application.Common;
using Ketabi.Application.DTOs.Chat;
using Ketabi.Core.Domain.Entities;

namespace Ketabi.Application.Mappings;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Message, MessageDto>()
           .ForMember(d => d.MessageId, o => o.MapFrom(s => s.Id))
           .ForMember(d => d.SenderName, o => o.MapFrom(s =>
               s.Sender != null
                   ? $"{s.Sender.FirstName} {s.Sender.LastName}"
                   : string.Empty))
           .ForMember(d => d.SenderAvatar, o => o.MapFrom(s =>
               s.Sender != null && s.Sender.ProfilePictureUrl != null
                   ? s.Sender.ProfilePictureUrl
                   : AppConstants.DefaultProfilePic))
           .ForMember(d => d.TimeAgo, o => o.MapFrom(s => GetTimeAgo(s.CreatedAt)))
           .ForMember(d => d.IsOwn, o => o.Ignore());

        // conversation mapping
        CreateMap<Conversation, ConversationDto>()
            .ForMember(d => d.ConversationId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.OwnerName, o => o.MapFrom(s =>
                s.Owner != null
                    ? $"{s.Owner.FirstName} {s.Owner.LastName}"
                    : string.Empty))
            .ForMember(d => d.OwnerAvatar, o => o.MapFrom(s =>
                s.Owner != null && s.Owner.ProfilePictureUrl != null
                    ? s.Owner.ProfilePictureUrl
                    : AppConstants.DefaultProfilePic))
            .ForMember(d => d.RequesterName, o => o.MapFrom(s =>
                s.Requester != null
                    ? $"{s.Requester.FirstName} {s.Requester.LastName}"
                    : string.Empty))
            .ForMember(d => d.RequesterAvatar, o => o.MapFrom(s =>
                s.Requester != null && s.Requester.ProfilePictureUrl != null
                    ? s.Requester.ProfilePictureUrl
                    : AppConstants.DefaultProfilePic))
            .ForMember(d => d.BookTitle, o => o.MapFrom(s =>
                s.Request != null && s.Request.Listing != null
                    ? s.Request.Listing.Title
                    : string.Empty))
            .ForMember(d => d.Messages, o => o.MapFrom(s => s.Messages))
            .ForMember(d => d.LastMessage, o => o.MapFrom(s =>
                s.Messages != null && s.Messages.Any()
                    ? s.Messages.OrderByDescending(m => m.CreatedAt).First()
                    : null))
            .ForMember(d => d.UnreadCount, o => o.Ignore())
            .ForMember(d => d.RequestType, o => o.MapFrom(s =>
                s.Request != null
                    ? s.Request.Type.ToString()
                    : string.Empty))
            .ForMember(d => d.BorrowDurationDays, o => o.MapFrom(s =>
                s.Request != null && s.Request.Listing != null
                    ? s.Request.Listing.SharingDurationInDays
                    : null));
    }

    private static string GetTimeAgo(DateTime dt)
    {
        var diff = DateTime.UtcNow - dt;

        if (diff.TotalSeconds < 60) return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        if (diff.TotalDays < 30) return $"{(int)(diff.TotalDays / 7)}w ago";

        return dt.ToString("MMM d");
    }
}
