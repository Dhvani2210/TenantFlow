using TenantFlow.Application.DTOs;

namespace TenantFlow.Application.Interfaces;

public interface IHubNotificationService
{
    Task NotifyTaskCreated(string tenantId, TaskDto task);
    Task NotifyTaskUpdated(string tenantId, TaskDto task);
    Task NotifyTaskDeleted(string tenantId, Guid taskId);
}