using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;

namespace DeliverySystem.Api.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly CepService _cepService;
    private readonly GeocodingService _geocodingService;
    private readonly NotificationService _notificationService;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        CepService cepService,
        GeocodingService geocodingService,
        NotificationService notificationService,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cepService = cepService;
        _geocodingService = geocodingService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<(CreateOrderResponse? Response, string? Error)> CreateAsync(CreateOrderRequest request, string userId)
    {
        var address = await _cepService.GetAddressAsync(
            request.DeliveryAddress.ZipCode,
            request.DeliveryAddress.Number);

        if (address is null)
            return (null, "CEP inválido ou não encontrado.");

        var coords = await _geocodingService.GeocodeAsync(address);
        if (coords.HasValue)
        {
            address.Latitude = coords.Value.Lat;
            address.Longitude = coords.Value.Lon;
        }

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            Description = request.Description,
            Value = request.Value,
            DeliveryAddress = address,
            UserId = userId
        };

        await _orderRepository.CreateAsync(order);

        await _notificationService.CreateAsync(
            userId,
            "ORDER_CREATED",
            $"Pedido {order.OrderNumber} criado com sucesso.");

        return (_mapper.Map<CreateOrderResponse>(order), null);
    }

    public async Task<IEnumerable<CreateOrderResponse>> GetByUserIdAsync(string userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CreateOrderResponse>>(orders);
    }

    private static string GenerateOrderNumber()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..5].ToUpper();
        return $"PED-{datePart}-{randomPart}";
    }
}
