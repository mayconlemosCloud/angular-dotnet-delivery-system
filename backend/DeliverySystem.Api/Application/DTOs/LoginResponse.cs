namespace DeliverySystem.Api.Application.DTOs;

public record LoginResponse(string Token, DateTime ExpiresAt);
