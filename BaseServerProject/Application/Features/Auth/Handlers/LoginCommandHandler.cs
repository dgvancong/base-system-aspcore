using MediatR;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Auth.Commands;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IDateTimeProvider dateTimeProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Step 1: Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found with email {Email}", request.Email);
                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            // Step 2: Check if user can login
            if (!user.CanLogin())
            {
                var remainingMinutes = user.LockoutEnd.HasValue
                    ? (int)(user.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes
                    : 0;

                _logger.LogWarning("Login failed: User {Email} is locked out. Remaining: {Minutes} minutes",
                    request.Email, remainingMinutes);

                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = $"Account is locked. Please try again in {remainingMinutes} minutes",
                    LockoutMinutesRemaining = remainingMinutes > 0 ? remainingMinutes : 0
                };
            }

            // Step 3: Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                user.RecordFailedLogin();
                await _userRepository.UpdateAsync(user, cancellationToken);

                var remainingAttempts = 5 - user.FailedLoginAttempts;
                _logger.LogWarning("Login failed: Invalid password for user {Email}. Attempts remaining: {Attempts}",
                    request.Email, remainingAttempts);

                return new LoginResult
                {
                    Success = false,
                    ErrorMessage = remainingAttempts > 0
                        ? $"Invalid password. {remainingAttempts} attempt(s) remaining"
                        : "Account has been locked due to too many failed attempts"
                };
            }

            // Step 4: Record successful login
            user.RecordSuccessfulLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Step 5: Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user.Id, request.IpAddress);

            // Step 6: Save refresh token
            await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);

            // Step 7: Prepare response
            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = 900, // 15 minutes
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FullName = user.GetFullName(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    LastLoginAt = user.LastLoginAt
                }
            };

            // Step 8: Log success
            _logger.LogInformation("User {Email} logged in successfully from IP {IpAddress}",
                user.Email, request.IpAddress);

            return new LoginResult
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email {Email}", request.Email);

            return new LoginResult
            {
                Success = false,
                ErrorMessage = "An error occurred during login. Please try again later."
            };
        }
    }
}