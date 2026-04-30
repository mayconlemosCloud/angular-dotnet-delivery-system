using System.Threading.Channels;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;
using DeliverySystem.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DeliverySystem.Api.Application.Services;

public class DeliveryTrackingService : BackgroundService
{
    public const double DefaultOriginLat = -23.5614;
    public const double DefaultOriginLon = -46.6564;

    private const int Steps      = 20;
    private const int IntervalMs = 3000;

    private readonly Channel<TrackingJob> _channel = Channel.CreateUnbounded<TrackingJob>();
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeliveryTrackingService> _logger;

    public DeliveryTrackingService(
        IHubContext<NotificationHub> hubContext,
        IServiceScopeFactory scopeFactory,
        ILogger<DeliveryTrackingService> logger)
    {
        _hubContext   = hubContext;
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    public void EnqueueTracking(TrackingJob job) => _channel.Writer.TryWrite(job);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
            _ = Task.Run(() => SimulateRouteAsync(job, stoppingToken), stoppingToken);
    }

    private async Task SimulateRouteAsync(TrackingJob job, CancellationToken ct)
    {
        _logger.LogInformation("Tracking started: {OrderNumber}", job.OrderNumber);

        for (var i = 1; i <= Steps; i++)
        {
            if (ct.IsCancellationRequested) break;

            var progress         = (double)i / Steps;
            var lat              = Lerp(job.OriginLat, job.DestLat, progress) + CurveJitter(progress);
            var lon              = Lerp(job.OriginLon, job.DestLon, progress) + CurveJitter(progress);
            var estimatedMinutes = (int)Math.Round((1 - progress) * 30);
            var isCompleted      = i == Steps;

            var update = new CourierLocationUpdate(job.OrderNumber, lat, lon, progress, estimatedMinutes, isCompleted);
            await _hubContext.Clients.User(job.UserId).SendAsync("CourierLocationUpdate", update, ct);

            if (isCompleted)
            {
                // Persist DELIVERED status in MongoDB using a scoped repository
                await UpdateDeliveryStatusAsync(job.OrderNumber, DeliveryStatus.Delivered);
                _logger.LogInformation("Tracking completed: {OrderNumber}", job.OrderNumber);
            }
            else
            {
                try { await Task.Delay(IntervalMs, ct); }
                catch (TaskCanceledException) { break; }
            }
        }
    }

    private async Task UpdateDeliveryStatusAsync(string orderNumber, string status)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IDeliveryRepository>();
        await repo.UpdateStatusAsync(orderNumber, status);
    }

    private static double Lerp(double from, double to, double t) => from + (to - from) * t;
    private static double CurveJitter(double t) => Math.Sin(t * Math.PI) * 0.003;
}
