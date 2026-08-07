using Microsoft.EntityFrameworkCore;
using jobFinderBackend.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
// Register JobFinderBackendDbContext as scoped for incoming HTTP requests
builder.Services.AddDbContext<JobFinderBackendDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("jobFinderBackendDatabase")
    )
);
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
