using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Runtime.CompilerServices;
using TenantFlow.Infrastructure.Repositories;
using TenantFlow.Infrastructure.Persistence;
using TenantFlow.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TenantFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));

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