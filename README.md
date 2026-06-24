# TenantFlow

A multi-tenant SaaS project management application built with ASP.NET Core, EF Core, SQL Server, SignalR, and React + TypeScript.

## Tech Stack

**Backend**: ASP.NET Core 9, Entity Framework Core, SQL Server 2022, SignalR, JWT Authentication
**Frontend**: React, TypeScript, Vite, Tailwind CSS
**Infrastructure**: Docker, Docker Compose

## Running with Docker

TenantFlow runs as a three-container stack: SQL Server, the ASP.NET Core API, and the React frontend served via Nginx.

### Prerequisites
- Docker Desktop (with at least 4GB memory allocated — SQL Server requires a minimum of 2GB)
- Copy `.env.example` to `.env` and fill in real values

### Running

\`\`\`bash
docker compose up
\`\`\`

This builds and starts all three services:
- `db` — SQL Server 2022
- `api` — ASP.NET Core 9 Web API (port 5253)
- `frontend` — React app served via Nginx (port 3000)

### Architecture notes

**Migrations run automatically on API startup**, via `dbContext.Database.Migrate()` in `Program.cs`, rather than the `dotnet ef` CLI. This is a deliberate choice: the final API image is built from the .NET runtime base image (not the SDK) to keep the production image small, and the SDK is required for `dotnet ef` commands. Calling `Database.Migrate()` programmatically uses the EF Core runtime library directly, requiring no SDK or CLI tooling.

This approach is appropriate for a single-instance deployment like this one. In a horizontally-scaled production environment with multiple API replicas starting simultaneously, this pattern can cause migration race conditions — a separate migration step (run once, before scaling up API instances) would be the safer choice at that scale.

**The API container connects to SQL Server using the Docker Compose service name (`db`), not `localhost`** — containers on the same Compose network resolve each other by service name.

**Secrets are managed via `.env`** (gitignored) rather than hardcoded in `docker-compose.yml`, following the same pattern used for `appsettings.Development.json`.

**Containerizing surfaced a real bug**: replaying the full migration history against a fresh database revealed two duplicate-column migrations that had gone unnoticed during incremental local development. `AddTenantIsolationColumns` was attempting to add `PasswordHash`, `Role`, and `TenantId` columns that `Baseline` already created — invisible locally because the local database had migrations applied incrementally, never replayed from zero. Fixed by editing the affected migration directly to remove the redundant operations, preserving its already-applied history rather than regenerating it.