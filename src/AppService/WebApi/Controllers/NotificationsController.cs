using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Notifications;
using YourBrand.Application.Notifications.Commands;
using YourBrand.Application.Notifications.Queries;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<ActionResult<NotificationsResults>> GetNotifications(
        bool includeUnreadNotificationsCount = false,
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetNotificationsQuery(includeUnreadNotificationsCount, page, pageSize, sortBy, sortDirection), cancellationToken));
    }

    [HttpPost("{id}/MarkAsRead")]
    public async Task<ActionResult> MarkNotificationAsRead(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);

        return Ok();
    }

    [HttpPost("MarkAllAsRead")]
    public async Task<ActionResult> MarkAllNotificationsAsRead(CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> CreateNotification(CreateNotificationDto createNotificationDto, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateNotificationCommand(
            createNotificationDto.Content,
            createNotificationDto.Link,
            createNotificationDto.UserId,
            createNotificationDto.ScheduledFor),
            cancellationToken);

        return Ok();
    }
}

public sealed record CreateNotificationDto(
    string Content,
    string? Link,
    string? UserId,
    DateTimeOffset? ScheduledFor
);
