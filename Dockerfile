FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TenantFlow.Api/TenantFlow.Api.csproj", "TenantFlow.Api/"]
COPY ["TenantFlow.Application/TenantFlow.Application.csproj", "TenantFlow.Application/"]
COPY ["TenantFlow.Domain/TenantFlow.Domain.csproj", "TenantFlow.Domain/"]
COPY ["TenantFlow.Infrastructure/TenantFlow.Infrastructure.csproj", "TenantFlow.Infrastructure/"]
RUN dotnet restore "TenantFlow.Api/TenantFlow.Api.csproj"
COPY . .
RUN dotnet publish "TenantFlow.Api/TenantFlow.Api.csproj" -c Release -o  /app/publish

FROM base as final
WORKDIR /app
COPY --from=build /app/publish .
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["bash", "entrypoint.sh"]
