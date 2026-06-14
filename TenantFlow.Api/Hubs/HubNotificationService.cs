using Microsoft.AspNetCore.SignalR;
using TenantFlow.Application.DTOs;
using TenantFlow.Application.Interfaces;

namespace TenantFlow.Api.Hubs;

public class HubNotificationService : IHubNotificationService
{
    private readonly IHubContext<TaskHub> _hubContext;

    public HubNotificationService(IHubContext<TaskHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyTaskCreated(string tenantId, TaskDto task)
    {
        await _hubContext.Clients.Group(tenantId).SendAsync("TaskCreated", task);
    }

    public async Task NotifyTaskUpdated(string tenantId, TaskDto task)
    {
        await _hubContext.Clients.Group(tenantId).SendAsync("TaskUpdated", task);
    }

    public async Task NotifyTaskDeleted(string tenantId, Guid taskId)
    {
        await _hubContext.Clients.Group(tenantId).SendAsync("TaskDeleted", taskId);
    }
}