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
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "dbo",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Derive the correct TenantId for each Task from its parent Project.
            // This ensures each Task is scoped to the same Tenant as the Project it belongs to,
            // rather than blindly assigning a hardcoded TenantId that would corrupt cross-tenant data.
            migrationBuilder.Sql(@"
                UPDATE t
                SET t.TenantId = p.TenantId
                FROM dbo.Tasks t
                INNER JOIN dbo.Projects p ON t.ProjectId = p.ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TenantId",
                schema: "dbo",
                table: "Tasks",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tenants_TenantId",
                schema: "dbo",
                table: "Tasks",
                column: "TenantId",
                principalSchema: "dbo",
                principalTable: "Tenants",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tenants_TenantId",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TenantId",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "dbo",
                table: "Tasks");
        }
    }
}
