using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Auth.Commands;
using BaseServerProject.Application.Features.Auth.DTOs;
using BaseServerProject.Core.Entities;
using BaseServerProject.Core.Enums;

namespace BaseServerProject.Application.Features.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        ILogger<RegisterCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        try
        {
            // Step 1: Check if email already exists
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingEmail != null)
            {
                errors.Add("Email is already registered");
            }

            // Step 2: Check if username already exists
            var existingUsername = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUsername != null)
            {
                errors.Add("Username is already taken");
            }

            // Step 3: Return errors if any
            if (errors.Any())
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Registration failed",
                    Errors = errors
                };
            }

            // Step 4: Create new user
            var user = new User
            {
                Id = 0,
                Email = request.Email.ToLower().Trim(),
                Username = request.Username.Trim(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                LastName = request.LastName?.Trim() ?? string.Empty,
                IsEmailConfirmed = false, // Can set to true if not using email confirmation
                Status = UserStatus.Active,
                CreatedAt = _dateTimeProvider.UtcNow,
                FailedLoginAttempts = 0
            };

            // Step 5: Save user to database
            await _userRepository.CreateAsync(user, cancellationToken);

            // Step 6: Log successful registration
            _logger.LogInformation("New user registered: {Email} from IP {IpAddress}",
                request.Email, request.IpAddress);

            // Step 7: Prepare response
            var response = new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.Username,
                Message = "Registration successful! Please login to continue.",
                RequiresEmailConfirmation = false, // Set to true if implementing email confirmation
                CreatedAt = user.CreatedAt
            };

            return new RegisterResult
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email {Email}", request.Email);

            return new RegisterResult
            {
                Success = false,
                ErrorMessage = "An error occurred during registration. Please try again later.",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}