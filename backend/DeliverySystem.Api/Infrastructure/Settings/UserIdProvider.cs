using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace DeliverySystem.Api.Infrastructure.Settings;

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
