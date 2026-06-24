using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIsolationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Derive the correct TenantId for each Task from its parent Project.
            // This ensures each Task is scoped to the same Tenant as the Project it belongs to,
            // rather than blindly assigning a hardcoded TenantId that would corrupt cross-tenant data.
            migrationBuilder.Sql(@"
                UPDATE t
                SET t.TenantId = p.TenantId
                FROM dbo.Tasks t
                INNER JOIN dbo.Projects p ON t.ProjectId = p.ProjectId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
