using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        var (response, error) = await _userService.RegisterAsync(request);

        if (error is not null)
            return Conflict(new { message = error });

        return CreatedAtAction(nameof(Register), new { id = response!.Id }, response);
    }
}
