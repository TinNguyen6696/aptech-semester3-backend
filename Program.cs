using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaLentShowcase.API.Infrastructure.Persistence;
using TaLentShowcase.API.Middleware;
using TaLentShowcase.API.Repositories;
using TaLentShowcase.API.Repositories.Interfaces;
using TaLentShowcase.API.Services;
using TaLentShowcase.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The supplied value is invalid."
                            : error.ErrorMessage)
                        .ToArray());

            return new BadRequestObjectResult(new
            {
                status = StatusCodes.Status400BadRequest,
                message = "Validation failed.",
                errors
            });
        };
    });

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileService, ProfileService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}

app.MapControllers();

app.Run();
