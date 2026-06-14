using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TenantFlow.Api.Hubs;

[Authorize]
public class TaskHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("TenantId")?.Value;

        if (!string.IsNullOrEmpty(tenantId)) { 
            await Groups.AddToGroupAsync(Context.ConnectionId, tenantId);
        }

        await base.OnConnectedAsync();
    }
}
