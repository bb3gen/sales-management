using FluentValidation;
using SalesManagement.Api.Features.Products.Requests;
using SalesManagement.Api.Shared.Validation;

namespace SalesManagement.Api.Features.Products.Validation;

public abstract class ProductValidatorBase<T>
    : AbstractValidator<T>
    where T : IProductRequest
{
    protected ProductValidatorBase()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithName("商品名")
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.UnitPrice)
            .NotEmpty()
            .WithName("単価")
            .WithMessage(ValidationMessages.Required);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.UnitPrice.HasValue)
            .WithName("単価")
            .WithMessage("{PropertyName}は0以上で入力してください。");

        RuleFor(x => x.Unit)
            .MaximumLength(20)
            .WithName("単位")
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithName("備考")
            .WithMessage(ValidationMessages.MaxLength);
    }
}