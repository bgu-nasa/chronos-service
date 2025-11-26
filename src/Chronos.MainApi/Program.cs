using Chronos.Data;
using Chronos.Data.Context;
using Chronos.MainApi.Auth.Configuration;
using Chronos.MainApi.Shared.Extensions;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
if (builder.Environment.IsLocal())
{
    // Remove this later and use psql
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("AppDbContext"));
}

// Tell the app to load config files from the AppSettings folder
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("AppSettings/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"AppSettings/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Register configurations
builder.Services.Configure<AuthConfiguration>(builder.Configuration.GetSection(nameof(AuthConfiguration)));

// Register repositories
builder.Services.AddServiceRepositories();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<OrganizationMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();