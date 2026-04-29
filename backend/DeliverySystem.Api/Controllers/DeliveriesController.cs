using System.Security.Claims;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.Api.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize]
public class DeliveriesController : ControllerBase
{
    private readonly DeliveryService _deliveryService;

    public DeliveriesController(DeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (response, error, statusCode) = await _deliveryService.CreateAsync(request, userId);

        return statusCode switch
        {
            404 => NotFound(new { message = error }),
            409 => Conflict(new { message = error }),
            _ => CreatedAtAction(nameof(Create), new { id = response!.Id }, response)
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var deliveries = await _deliveryService.GetByUserIdAsync(userId);
        return Ok(deliveries);
    }
}
