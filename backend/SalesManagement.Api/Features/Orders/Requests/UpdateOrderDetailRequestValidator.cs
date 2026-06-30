using FluentValidation;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class UpdateOrderDetailRequestValidator
    : AbstractValidator<UpdateOrderDetailRequest>
{
    public UpdateOrderDetailRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithName("商品")
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithName("数量")
            .WithMessage(ValidationMessages.GreaterThan);
    }
}
