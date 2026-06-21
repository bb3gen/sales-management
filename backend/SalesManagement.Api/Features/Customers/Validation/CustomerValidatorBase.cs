using FluentValidation;

using SalesManagement.Api.Shared.Validation;
using SalesManagement.Api.Features.Customers.Requests;

namespace SalesManagement.Api.Features.Customers.Validation;
public abstract class CustomerValidatorBase<T>
    : AbstractValidator<T>
    where T : ICustomerRequest
{
    protected CustomerValidatorBase()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithName("顧客名")
            .WithMessage(ValidationMessages.Required)
            .MaximumLength(100)
            .WithMessage(ValidationMessages.MaxLength);

        RuleFor(x => x.CustomerKana)
            .MaximumLength(100)
            .WithName("顧客名カナ")
            .WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerKana));

        RuleFor(x => x.PostalCode)
            .MaximumLength(8)
            .WithName("郵便番号")
            .WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .WithName("住所")
            .WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .WithName("電話番号")
            .WithMessage(ValidationMessages.MaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithName("メールアドレス")
            .WithMessage(ValidationMessages.Email)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}