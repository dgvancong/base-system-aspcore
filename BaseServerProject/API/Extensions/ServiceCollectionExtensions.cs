using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Auth.Validators;
using BaseServerProject.Infrastructure.Persistence;
using BaseServerProject.Infrastructure.Persistence.Repositories;
using BaseServerProject.Infrastructure.Services;
using BaseServerProject.API.Middleware;

namespace BaseServerProject.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandValidator).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Controllers
        services.AddControllers();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // HttpContext
        services.AddHttpContextAccessor();

        return services;
    }

    public static IApplicationBuilder UseApiMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}