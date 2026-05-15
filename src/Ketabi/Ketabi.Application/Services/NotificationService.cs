using AutoMapper;
using Ketabi.Application.DTOs.Notifications;
using Ketabi.Application.Interfaces;
using Ketabi.Core.Domain.Entities;
using Ketabi.Core.Interfaces;

namespace Ketabi.Application.Services;

internal class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(Guid userId, int page, int pageSize)
    {
        var notifications = await _unitOfWork.Notifications.GetNotificationsForUserAsync(userId, page, pageSize);
        return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
    }

    public Task<int> GetUnreadCountAsync(Guid userId)
        => _unitOfWork.Notifications.GetUnreadCountAsync(userId);

    public Task<int> GetTotalCountAsync(Guid userId)
        => _unitOfWork.Notifications.GetTotalCountAsync(userId);

    public Task MarkAllAsReadAsync(Guid userId)
        => _unitOfWork.Notifications.MarkAllAsReadAsync(userId);

    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null || notification.UserId != userId) return;

        notification.IsRead = true;
        notification.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        var entity = _mapper.Map<Notification>(dto);
        await _unitOfWork.Notifications.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
