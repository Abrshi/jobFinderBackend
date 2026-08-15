using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Scalar.AspNetCore;
using Asp.Versioning;

using jobFinderBackend.Infrastructure.Data;

using jobFinderBackend.Application.Interfaces;
using jobFinderBackend.Api.Services;
using jobFinderBackend.Application.Validators;
using jobFinderBackend.Application.Users.Commands;

using jobFinderBackend.Infrastructure.Persistence.Repositories;
using jobFinderBackend.Infrastructure.Security;
using jobFinderBackend.Infrastructure.Persistence.Configuration.Data;


using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IJobPlatformRepository, JobPlatformRepository>();
// Database
builder.Services.AddDbContext<JobFinderBackendDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "jobFinderBackendDatabase"
        )
    ));

// OpenAPI
builder.Services.AddOpenApi();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepository>();
builder.Services.AddScoped<IUserJobPlatformRepository, UserJobPlatformRepository>();

// Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Current User
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

// MediatR / CQRS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(RegisterUserCommand).Assembly
    );
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT key is not configured."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateIssuer = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,

            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };

        // Read JWT from HttpOnly cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["auth_token"];

                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5000",
                "https://localhost:5000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.AssumeDefaultVersionWhenUnspecified = false;

    options.ReportApiVersions = true;

    options.ApiVersionReader =
        new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";

    options.SubstituteApiVersionInUrl = true;
});


var app = builder.Build();

// OpenAPI / Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

// Data Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<JobFinderBackendDbContext>();

    await DataSeeder.SeedAsync(context);
}

// HTTP Request Pipeline
app.UseHttpsRedirection();


// CORS must be before Authentication / Authorization
app.UseCors("AllowFrontend");


// Authentication must come before Authorization
app.UseAuthentication();

app.UseAuthorization();

// Controllers
app.MapControllers();


app.Run();