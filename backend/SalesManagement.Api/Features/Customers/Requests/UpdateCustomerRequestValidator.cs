using SalesManagement.Api.Features.Customers.Validation;

namespace SalesManagement.Api.Features.Customers.Requests;

public sealed class UpdateCustomerRequestValidator
    : CustomerValidatorBase<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        // 差分が必要な場合
        // RuleFor(x => x.CustomerName)
        //     .NotEmpty()
        //     .WithMessage("顧客名を入力してください")
        //     .MaximumLength(100);

    }
}
