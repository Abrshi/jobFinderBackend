using Microsoft.EntityFrameworkCore;
using FluentValidation;

using jobFinderBackend.Infrastructure.Data;

using jobFinder.Application.Interfaces;
using jobFinder.Application.Services;
using jobFinder.Application.Validators;

using jobFinder.Infrastructure.Persistence.Repositories;
using jobFinder.Infrastructure.Security;
using jobFinderBackend.Infrastructure.Persistence.Configuration.Data;


var builder = WebApplication.CreateBuilder(args);


// Database
builder.Services.AddDbContext<JobFinderBackendDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("jobFinderBackendDatabase")
    ));


// Application services
builder.Services.AddScoped<IAuthService, AuthService>();


// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();


// Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


// Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();


// Controllers
builder.Services.AddControllers();


var app = builder.Build();
// data seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<JobFinderBackendDbContext>();

    await DataSeeder.SeedAsync(context);
}


// HTTP request pipeline

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();