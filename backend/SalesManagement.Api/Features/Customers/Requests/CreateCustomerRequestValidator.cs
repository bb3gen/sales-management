using SalesManagement.Api.Features.Customers.Validation;

namespace SalesManagement.Api.Features.Customers.Requests;

public sealed class CreateCustomerRequestValidator
    : CustomerValidatorBase<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        // 差分が必要な場合に定義
        // RuleFor(x => x.CustomerName)
        //     .NotEmpty()
        //     .WithName("顧客名")
        //     .WithMessage(ValidationMessages.Required)
        //     .MaximumLength(100)
        //     .WithMessage(ValidationMessages.MaxLength);
    }
}
