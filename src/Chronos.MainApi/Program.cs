using System.Text;
using Chronos.Data;
using Chronos.Data.Context;
using Chronos.MainApi.Auth;
using Chronos.MainApi.Auth.Configuration;
using Chronos.MainApi.Management;
using Chronos.MainApi.Shared.Extensions;
using Chronos.MainApi.Shared.Middleware;
using Chronos.MainApi.Shared.Middleware.Rbac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

// Register repositories
builder.Services.AddServiceRepositories();

// Register modules
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddManagementModule(builder.Configuration);

// Configure JWT Authentication
var authConfig = builder.Configuration.GetSection(nameof(AuthConfiguration)).Get<AuthConfiguration>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig!.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = authConfig.Issuer,
        ValidateAudience = true,
        ValidAudience = authConfig.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, RequireRoleOrgHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, RequireRoleDeptHandler>();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsLocal())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<OrganizationMiddleware>();

// app.UseHttpsRedirection();
app.MapControllers();

app.Run();