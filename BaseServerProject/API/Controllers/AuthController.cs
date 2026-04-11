using MediatR;
using Microsoft.AspNetCore.Mvc;
using BaseServerProject.Application.Features.Auth.Commands;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        // Get client IP
        var ipAddress = GetIpAddress();

        // Create command
        var command = new RegisterCommand
        {
            Email = request.Email,
            Username = request.Username,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            IpAddress = ipAddress
        };

        // Execute
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            if (result.Errors != null && result.Errors.Any())
            {
                return Conflict(new
                {
                    success = false,
                    message = result.ErrorMessage,
                    errors = result.Errors
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.ErrorMessage
            });
        }

        return Ok(new
        {
            success = true,
            message = result.Data?.Message,
            data = result.Data
        });
    }

    /// <returns>Access token and refresh token</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Get client information
        var ipAddress = GetIpAddress();
        var userAgent = Request.Headers["User-Agent"].ToString();

        // Create command
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            RememberMe = request.RememberMe
        };

        // Execute
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            if (result.LockoutMinutesRemaining.HasValue)
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = result.ErrorMessage,
                    lockoutRemaining = result.LockoutMinutesRemaining
                });
            }

            return Unauthorized(new { success = false, message = result.ErrorMessage });
        }

        // Set refresh token as HTTP-only cookie if Remember Me
        if (request.RememberMe)
        {
            SetRefreshTokenCookie(result.Data!.RefreshToken);
        }

        return Ok(new
        {
            success = true,
            message = "Login successful",
            data = result.Data
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Remove refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { success = true, message = "Logged out successfully" });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken,
            IpAddress = GetIpAddress()
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return Unauthorized(new { success = false, message = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            data = result.Data
        });
    }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"].ToString();

        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/auth"
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}

// DTO cho refresh token
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}