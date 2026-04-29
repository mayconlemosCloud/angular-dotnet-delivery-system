using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DeliverySystem.Api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}
