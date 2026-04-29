using DeliverySystem.Api.Application.DTOs;
using FluentValidation;

namespace DeliverySystem.Api.Application.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória.")
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Valor deve ser maior que zero.");

        RuleFor(x => x.DeliveryAddress)
            .NotNull().WithMessage("Endereço de entrega é obrigatório.")
            .ChildRules(address =>
            {
                address.RuleFor(a => a.ZipCode)
                    .NotEmpty().WithMessage("CEP é obrigatório.")
                    .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido. Formato esperado: 00000-000.");

                address.RuleFor(a => a.Number)
                    .NotEmpty().WithMessage("Número é obrigatório.");
            });
    }
}
