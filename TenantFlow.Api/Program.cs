using Microsoft.EntityFrameworkCore;
using TenantFlow.Api.Data;
using Scalar.AspNetCore;
using System.Runtime.CompilerServices;
using TenantFlow.Api.Repositories;
using TenantFlow.Api.Repositories.Interfaces;

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