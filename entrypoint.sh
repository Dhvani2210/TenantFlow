#!/bin/bash
set -e

echo "Waiting for PostgreSQL to be ready..."

until (echo > /dev/tcp/db/5432) 2>/dev/null; do
  echo "PostgreSQL not ready yet - waiting 5 seconds..."
  sleep 5
done

echo "PostgreSQL is ready. Waiting 5 more seconds for full initialisation..."
sleep 5

echo "Starting API..."
exec dotnet TenantFlow.Api.dll