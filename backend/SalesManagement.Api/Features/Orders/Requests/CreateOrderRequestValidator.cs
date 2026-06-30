using FluentValidation;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Orders.Requests;

public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.OrderDate)
            .NotEmpty()
            .WithName("受注日")
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithName("顧客")
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.Details)
            .NotEmpty()
            .WithMessage("明細を1件以上入力してください。");

        RuleForEach(x => x.Details)
            .SetValidator(new CreateOrderDetailRequestValidator());
    }
}