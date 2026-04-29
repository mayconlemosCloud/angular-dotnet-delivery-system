using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;

namespace DeliverySystem.Api.Application.Services;

public class DeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        IMapper mapper)
    {
        _deliveryRepository = deliveryRepository;
        _orderRepository = orderRepository;
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

        return (_mapper.Map<CreateDeliveryResponse>(delivery), null, 201);
    }

    public async Task<IEnumerable<CreateDeliveryResponse>> GetByUserIdAsync(string userId)
    {
        var deliveries = await _deliveryRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CreateDeliveryResponse>>(deliveries);
    }
}
