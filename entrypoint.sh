#!/bin/bash
set -e

echo "Waiting for SQL Server to be ready..."

until (echo > /dev/tcp/db/1433) 2>/dev/null; do
  echo "SQL Server not ready yet - waiting 5 seconds..."
  sleep 5
done

echo "SQL Server is ready. Waiting 5 more seconds for full initialisation..."
sleep 5

echo "Starting API..."
exec dotnet TenantFlow.Api.dll