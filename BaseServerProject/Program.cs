using BaseServerProject.API.Extensions;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Auth.Handlers;
using BaseServerProject.Application.Features.Orders.Handlers;
using BaseServerProject.Application.Features.Products.Handlers;
using BaseServerProject.Infrastructure.Persistence;
using BaseServerProject.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();


// Đăng ký Product Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<LoginCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<RegisterCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<RefreshTokenCommandHandler>();

    cfg.RegisterServicesFromAssemblyContaining<CreateProductCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetProductListQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<DeleteProductCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetProductByIdQueryHandler>();

    cfg.RegisterServicesFromAssemblyContaining<CreateOrderCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetOrderListQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetOrderByIdQueryHandler>();
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApiMiddleware();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        await dbContext.Database.CanConnectAsync();
        Console.WriteLine("✅ Database connection successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
    }
}

app.Run();