using Microsoft.EntityFrameworkCore;
using TenantFlow.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register TenantFlowDbContext with the DI container
// AddDbContext tells ASP.NET Core: when something asks for a TenantFlowDbContext,
// create one using SQL Server with this connection string
builder.Services.AddDbContext<TenantFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.Run();

