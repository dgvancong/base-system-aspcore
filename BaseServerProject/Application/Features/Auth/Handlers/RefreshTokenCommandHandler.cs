using MediatR;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Auth.Commands;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IDateTimeProvider dateTimeProvider,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Tìm refresh token trong database
            var refreshToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found: {Token}", request.RefreshToken);
                return new RefreshTokenResult
                {
                    Success = false,
                    ErrorMessage = "Invalid refresh token"
                };
            }

            // 2. Kiểm tra token có còn hiệu lực không
            if (!refreshToken.IsActive)
            {
                _logger.LogWarning("Refresh token is expired or revoked: {Token}", request.RefreshToken);
                return new RefreshTokenResult
                {
                    Success = false,
                    ErrorMessage = "Refresh token has expired"
                };
            }

            // 3. Lấy user từ refresh token
            var user = refreshToken.User;
            if (user == null)
            {
                user = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
            }

            if (user == null)
            {
                return new RefreshTokenResult
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            // 4. Revoke old refresh token
            await _userRepository.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

            // 5. Generate new tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id, request.IpAddress);

            // 6. Save new refresh token
            await _userRepository.AddRefreshTokenAsync(newRefreshToken, cancellationToken);

            // 7. Prepare response
            var response = new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = 900,
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

            _logger.LogInformation("Refresh token renewed for user: {Email}", user.Email);

            return new RefreshTokenResult
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token");
            return new RefreshTokenResult
            {
                Success = false,
                ErrorMessage = "An error occurred during token refresh"
            };
        }
    }
}