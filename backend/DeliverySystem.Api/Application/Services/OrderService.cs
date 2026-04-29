using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;
using DeliverySystem.Api.Domain.Interfaces;

namespace DeliverySystem.Api.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<CreateOrderResponse> CreateAsync(CreateOrderRequest request, string userId)
    {
        var order = _mapper.Map<Order>(request);
        order.UserId = userId;
        order.OrderNumber = GenerateOrderNumber();

        await _orderRepository.CreateAsync(order);

        return _mapper.Map<CreateOrderResponse>(order);
    }

    public async Task<IEnumerable<CreateOrderResponse>> GetByUserIdAsync(string userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CreateOrderResponse>>(orders);
    }

    private static string GenerateOrderNumber()
    {
        // PED-YYYYMMDD-XXXXX (ex: PED-20260429-A3F9C)
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..5].ToUpper();
        return $"PED-{datePart}-{randomPart}";
    }
}
