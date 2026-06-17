
namespace TenantFlow.Application.DTOs
{
    public class InviteMemberDto
    {
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
    }
}
