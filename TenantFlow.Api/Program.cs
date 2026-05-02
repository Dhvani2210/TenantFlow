using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Runtime.CompilerServices;
using TenantFlow.Infrastructure.Repositories;
using TenantFlow.Infrastructure;


using TenantFlow.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();