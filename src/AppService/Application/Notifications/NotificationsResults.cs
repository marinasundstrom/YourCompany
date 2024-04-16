using YourBrand.Application.Common.Models;

namespace YourBrand.Application.Notifications;

public record class NotificationsResults(IEnumerable<NotificationDto> Items, int? UnreadNotificationsCount, int TotalCount)
    : ItemResult<NotificationDto>(Items, TotalCount);