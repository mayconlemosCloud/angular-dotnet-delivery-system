using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;

namespace DeliverySystem.Api.Application.Services;

public class DeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly NotificationService _notificationService;
    private readonly DeliveryTrackingService _trackingService;
    private readonly IMapper _mapper;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        NotificationService notificationService,
        DeliveryTrackingService trackingService,
        IMapper mapper)
    {
        _deliveryRepository = deliveryRepository;
        _orderRepository = orderRepository;
        _notificationService = notificationService;
        _trackingService = trackingService;
        _mapper = mapper;
    }

    public async Task<(CreateDeliveryResponse? Response, string? Error, int StatusCode)> CreateAsync(
        CreateDeliveryRequest request, string userId)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(request.OrderNumber);
        if (order is null)
            return (null, "Pedido não encontrado.", 404);

        var existing = await _deliveryRepository.GetByOrderNumberAsync(request.OrderNumber);
        if (existing is not null)
            return (null, "Entrega já registrada para este pedido.", 409);

        var delivery = new Delivery
        {
            OrderNumber = request.OrderNumber,
            DeliveryDateTime = request.DeliveryDateTime,
            UserId = userId
        };

        await _deliveryRepository.CreateAsync(delivery);

        await _notificationService.CreateAsync(
            userId,
            "DELIVERY_REGISTERED",
            $"Entrega do pedido {delivery.OrderNumber} registrada para {delivery.DeliveryDateTime:dd/MM/yyyy HH:mm}.");

        // Use geocoded coordinates when available; fallback to a small offset from depot
        var destLat = order.DeliveryAddress.Latitude
            ?? DeliveryTrackingService.DefaultOriginLat + 0.05;
        var destLon = order.DeliveryAddress.Longitude
            ?? DeliveryTrackingService.DefaultOriginLon + 0.05;

        _trackingService.EnqueueTracking(new TrackingJob(
            delivery.OrderNumber,
            userId,
            DeliveryTrackingService.DefaultOriginLat,
            DeliveryTrackingService.DefaultOriginLon,
            destLat,
            destLon));

        return (_mapper.Map<CreateDeliveryResponse>(delivery), null, 201);
    }

    public async Task<IEnumerable<CreateDeliveryResponse>> GetByUserIdAsync(string userId)
    {
        var deliveries = await _deliveryRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CreateDeliveryResponse>>(deliveries);
    }
}
