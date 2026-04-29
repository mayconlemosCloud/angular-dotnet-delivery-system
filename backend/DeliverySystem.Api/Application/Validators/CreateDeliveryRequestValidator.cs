using DeliverySystem.Api.Application.DTOs;
using FluentValidation;

namespace DeliverySystem.Api.Application.Validators;

public class CreateDeliveryRequestValidator : AbstractValidator<CreateDeliveryRequest>
{
    public CreateDeliveryRequestValidator()
    {
        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Número do pedido é obrigatório.");

        RuleFor(x => x.DeliveryDateTime)
            .NotEmpty().WithMessage("Data/hora de entrega é obrigatória.");
    }
}
