using AutoMapper;
using DeliverySystem.Api.Application.DTOs;
using DeliverySystem.Api.Domain.Entities;

namespace DeliverySystem.Api.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
        CreateMap<User, CreateUserResponse>();

        // Address
        CreateMap<AddressRequest, Address>();
        CreateMap<Address, AddressResponse>();

        // Order
        CreateMap<CreateOrderRequest, Order>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        CreateMap<Order, CreateOrderResponse>();
    }
}
