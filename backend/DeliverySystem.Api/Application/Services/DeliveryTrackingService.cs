using System.Threading.Channels;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DeliverySystem.Api.Application.Services;

public class DeliveryTrackingService : BackgroundService
{
    // Default depot: Av. Paulista, São Paulo — used as origin for all simulations
    public const double DefaultOriginLat = -23.5614;
    public const double DefaultOriginLon = -46.6564;

    private const int Steps = 20;
    private const int IntervalMs = 3000;

    private readonly Channel<TrackingJob> _channel = Channel.CreateUnbounded<TrackingJob>();
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<DeliveryTrackingService> _logger;

    public DeliveryTrackingService(
        IHubContext<NotificationHub> hubContext,
        ILogger<DeliveryTrackingService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public void EnqueueTracking(TrackingJob job) => _channel.Writer.TryWrite(job);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            // Fire-and-forget each job so multiple deliveries run in parallel
            _ = Task.Run(() => SimulateRouteAsync(job, stoppingToken), stoppingToken);
        }
    }

    private async Task SimulateRouteAsync(TrackingJob job, CancellationToken ct)
    {
        _logger.LogInformation("Tracking simulation started: {OrderNumber}", job.OrderNumber);

        for (var i = 1; i <= Steps; i++)
        {
            if (ct.IsCancellationRequested) break;

            var progress = (double)i / Steps;
            var lat = Lerp(job.OriginLat, job.DestLat, progress) + CurveJitter(progress);
            var lon = Lerp(job.OriginLon, job.DestLon, progress) + CurveJitter(progress);
            var estimatedMinutes = (int)Math.Round((1 - progress) * 30);
            var isCompleted = i == Steps;

            var update = new CourierLocationUpdate(
                job.OrderNumber, lat, lon, progress, estimatedMinutes, isCompleted);

            await _hubContext.Clients.User(job.UserId)
                .SendAsync("CourierLocationUpdate", update, ct);

            if (!isCompleted)
            {
                try { await Task.Delay(IntervalMs, ct); }
                catch (TaskCanceledException) { break; }
            }
        }

        _logger.LogInformation("Tracking simulation completed: {OrderNumber}", job.OrderNumber);
    }

    private static double Lerp(double from, double to, double t) => from + (to - from) * t;

    // Adds a slight arc to the route so it doesn't look like a straight GPS line
    private static double CurveJitter(double t) => Math.Sin(t * Math.PI) * 0.003;
}
