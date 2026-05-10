namespace TenantFlow.Application.DTOs;

// Response DTO — what the API returns for any task endpoint.
// TenantId is excluded: it's an internal isolation column the frontend never needs.
public class TaskDto
{
    public Guid TaskId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } 
    public DateTime? DueDate { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

// Inbound DTO for creating a task. ProjectId is here because the caller
// declares which project this task belongs to. The service will validate
// that the project actually belongs to the current tenant before inserting.
public class CreateTaskDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? AssignedToUserId { get; init; }
}

// Inbound DTO for updating a task. ProjectId is intentionally absent —
// reassigning a task to a different project is not a supported operation.
// Status is included because progressing a task through its lifecycle
// (e.g. ToDo → InProgress → Done) is a core update operation.
public class UpdateTaskDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public Guid? AssignedToUserId { get; init; }
}
